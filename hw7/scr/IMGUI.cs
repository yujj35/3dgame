using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMGUI : MonoBehaviour
{

	public float health;//当前血量
	private float res;//变化到的目标血量
	public Slider healthSlider; //对应UGUI的血条
	private float healthEps;//血条变化精度
	void Start()
	{

		health = 1.0f;//当前血量
		res = 1.0f;//变化到的目标血量
		healthEps = 0.1f;//血条变化精度
	}
	void OnGUI()
	{

		if (GUI.Button(new Rect(255, 20, 40, 20), "+"))
		{
			//点击增加血量
			if (res + healthEps > 1.0f) //超出最大值
				res = 1.0f;//设置为最大值
			else
				res = res + healthEps;//否则增加一个量
		}
		if (GUI.Button(new Rect(310, 20, 40, 20), "-"))
		{
			//点击减少血量
			if (res - healthEps < 0.0f) //低于最小值
				res = 0.0f;//设置为最小值
			else
				res = res - healthEps;//否则减少一个量
		}
		health = Mathf.Lerp(health, res, 0.05f);//进行插值
		if (health > 0.7f)//根据当前血量的多少决定绘制血条的颜色
			GUI.color = Color.green;
		else if (health > 0.4f)
			GUI.color = Color.yellow;
		else
			GUI.color = Color.red;
		GUI.HorizontalScrollbar(new Rect(20, 20, 200, 20), 0.0f, health, 0.0f, 1.0f);//使用水平滚动条绘制血条 将其宽度作为血条的显示值
		if (healthSlider != null) //如果存在对应UGUI的血条
			healthSlider.value = health * 100;//修改对应UGUI的血条
	}
}
