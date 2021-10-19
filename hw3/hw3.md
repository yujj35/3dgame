# 牧师与魔鬼 动作分离版

## 要求
在该游戏原有代码的基础上，实现动作及动作管理器与其他部分的分离。

动作分离版的改进目的主要有以下两点：

将角色的动作（上船、上岸）和船的动作（开往对岸）分离出来交给动作管理器管理

增加裁判类，当游戏达到结束条件时，通知场景控制器游戏结束

## 代码
1、动作基类（SSAction）

```public class SSAction : ScriptableObject
    {
        public bool enable = true;
        public bool destroy = false;

        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }
        public ISSActionCallback callback { get; set; }

        protected SSAction()
        {

        }

        // Start is called before the first frame update
        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        // Update is called once per frame
        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }
 ```


2、简单动作实现

实现具体动作，将一个物体移动到目标位置，并通知任务完成：
```
 public class CCMoveToAction : SSAction
    {
        //目的地
        public Vector3 target;
        //速度
        public float speed;

        private CCMoveToAction()
        {

        }

        public static CCMoveToAction GetSSAction(Vector3 target, float speed)
        {
            CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
            action.target = target;
            action.speed = speed;
            return action;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            
        }

        // Update is called once per frame
        public override void Update()
        {
            //判断是否符合移动条件
            if (this.gameObject == null || this.transform.localPosition == target)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this);
                return;
            }
            //移动
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, target, speed * Time.deltaTime);
        }
    }
```

3、顺序动作组合类实现

实现一个动作组合序列，顺序播放动作：
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSequenceAction : SSAction, ISSActionCallback
{
    //动作序列
    public List<SSAction> sequence;
    //重复次数
    public int repeat = -1;
    //动作开始指针
    public int start = 0;

    //生产函数(工厂模式)
    public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence)
    {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.repeat = repeat;
        action.start = start;
        action.sequence = sequence;
        return action;
    }

    //对序列中的动作进行初始化
    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameObject = this.gameObject;
            action.transform = this.transform;
            action.callback = this;
            action.Start();
        }
    }

    //运行序列中的动作
    public override void Update()
    {
        if (sequence.Count == 0)
            return;
        if (start < sequence.Count)
        {
            sequence[start].Update();
        }
    }

    //回调处理，当有动作完成时触发
    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competed,
        int Param = 0,
        string strParam = null,
        Object objectParam = null)
    {
        source.destroy = false;
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0)
                repeat--;
            if (repeat == 0)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        }
    }

    void OnDestroy()
    {

    }
}
```

4、动作事件接口定义

接口作为接收通知对象的抽象类型


5、动作管理基类 – SSActionManager

这是动作对象管理器的基类，实现了所有动作的基本管理
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSActionManager : MonoBehaviour
{
  
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();

    private List<SSAction> waitingAdd = new List<SSAction>();

    private List<int> waitingDelete = new List<int>();

    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
            actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            Destroy(ac);
        }
        waitingDelete.Clear();
    }
    public void RunAction(GameObject gameObject, SSAction action, ISSActionCallback manager)
    {
        action.gameObject = gameObject;
        action.transform = gameObject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    // Start is called before the first frame update
    protected void Start()
    {

    }

}
```

6.裁判类
```
public class JudgeController : MonoBehaviour
{
    public FirstController mainController;
    public Shore leftShoreModel;
    public Shore rightShoreModel;
    public Boat boatModel;
    // Start is called before the first frame update
    void Start()
    {
        mainController = (FirstController)SSDirector.GetInstance().CurrentSceneController;
        this.leftShoreModel = mainController.leftShoreController.GetShore();
        this.rightShoreModel = mainController.rightShoreController.GetShore();
        this.boatModel = mainController.boatController.GetBoatModel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainController.isRunning)
            return;
        if (mainController.time <= 0)
        {
            mainController.JudgeCallback(false, "Game Over!");
            return;
        }
        this.gameObject.GetComponent<UserGUI>().gameMessage = "";
        //判断是否已经胜利
        if (rightShoreModel.priestCount == 3)
        {
            mainController.JudgeCallback(false, "You Win!");
            return;
        }
        else
        {
            
            int leftPriestNum, leftDevilNum, rightPriestNum, rightDevilNum;
            leftPriestNum = leftShoreModel.priestCount + (boatModel.isRight ? 0 : boatModel.priestCount);
            leftDevilNum = leftShoreModel.devilCount + (boatModel.isRight ? 0 : boatModel.devilCount);
            if (leftPriestNum != 0 && leftPriestNum < leftDevilNum)
            {
                mainController.JudgeCallback(false, "Game Over!");
                return;
            }
            rightPriestNum = rightShoreModel.priestCount + (boatModel.isRight ? boatModel.priestCount : 0);
            rightDevilNum = rightShoreModel.devilCount + (boatModel.isRight ? boatModel.devilCount : 0);
            if (rightPriestNum != 0 && rightPriestNum < rightDevilNum)
            {
                mainController.JudgeCallback(false, "Game Over!");
                return;
            }
        }
    }
}
```
