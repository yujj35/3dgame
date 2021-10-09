# 实验报告 

## 实验背景

牧师与恶魔需要从岸的一端到达另一端，河上只有一条船，一条船只能坐两个角色，并且至少需要一个角色在船上船才可以行驶。并且，如果在某一侧（包括岸上和船上），恶魔的数量大于牧师的数量，牧师就会被恶魔吃掉（如果仅有恶魔则无事发生），游戏失败。玩家要安排牧师与恶魔的过河顺序，让牧师与恶魔全部到达另一边岸上，才能游戏通关。
## 实验要求

请将游戏中对象做成预制 

在GenGameObjects中创建长方形、正方形、球及其色彩代表游戏中的对象。

使用C#集合类型有效组织对象

整个游戏仅主摄像机和一个 Empty 对象，其他对象必须代码动态生成！！！ 。 整个游戏不许出现 Find 游戏对象， SendMessage这类突破程序结构的 通讯耦合 语句违背本条准则，不给分

请使用课件架构图编程，不接受非 MVC 结构程序

注意细节，例如：船未靠岸，牧师与魔鬼上下船运动中，均不能接受用户事件！

## MVC
模型（Model）：数据对象及关系  

· 游戏对象、空间关系

控制器（Controller）：接受用户事件，控制模型的变化

· 一个场景一个主控制器

· 至少实现与玩家交互的接口（IPlayerAction）

· 实现或管理运动

界面（View）：显示模型，将人机交互事件交给控制器处理

· 处收Input 事件

· 渲染GUI ，接收事件

## 代码分析

###

### UserGUI:设置UI和接收用户交互。

```
public class UserGUI : MonoBehaviour
{
    private IUserAction userAction;
    public string gameMessage;
    public int time;
    void Start()
    {
        time = 60;
        userAction = SSDirector.GetInstance().CurrentSenceController as IUserAction;
    }

    void OnGUI()
    {
        userAction.Check();
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 30;

        GUIStyle bigStyle = new GUIStyle();
        bigStyle.normal.textColor = Color.white;
        bigStyle.fontSize = 50;

        GUI.Label(new Rect(200, 30, 50, 200), "Priests and Devils", bigStyle);

        GUI.Label(new Rect(320, 100, 50, 200), gameMessage, style);

        GUI.Label(new Rect(0, 0, 100, 50), "Time: " + time, style);

        if(GUI.Button(new Rect(340, 160, 100, 50), "Restart"))
        {
            userAction.Restart();
        }
    }
}

```


具体代码可见源文件


