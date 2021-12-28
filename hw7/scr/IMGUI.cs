using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMGUI : MonoBehaviour
{

	public float health;//��ǰѪ��
	private float res;//�仯����Ŀ��Ѫ��
	public Slider healthSlider; //��ӦUGUI��Ѫ��
	private float healthEps;//Ѫ���仯����
	void Start()
	{

		health = 1.0f;//��ǰѪ��
		res = 1.0f;//�仯����Ŀ��Ѫ��
		healthEps = 0.1f;//Ѫ���仯����
	}
	void OnGUI()
	{

		if (GUI.Button(new Rect(255, 20, 40, 20), "+"))
		{
			//�������Ѫ��
			if (res + healthEps > 1.0f) //�������ֵ
				res = 1.0f;//����Ϊ���ֵ
			else
				res = res + healthEps;//��������һ����
		}
		if (GUI.Button(new Rect(310, 20, 40, 20), "-"))
		{
			//�������Ѫ��
			if (res - healthEps < 0.0f) //������Сֵ
				res = 0.0f;//����Ϊ��Сֵ
			else
				res = res - healthEps;//�������һ����
		}
		health = Mathf.Lerp(health, res, 0.05f);//���в�ֵ
		if (health > 0.7f)//���ݵ�ǰѪ���Ķ��پ�������Ѫ������ɫ
			GUI.color = Color.green;
		else if (health > 0.4f)
			GUI.color = Color.yellow;
		else
			GUI.color = Color.red;
		GUI.HorizontalScrollbar(new Rect(20, 20, 200, 20), 0.0f, health, 0.0f, 1.0f);//ʹ��ˮƽ����������Ѫ�� ��������ΪѪ������ʾֵ
		if (healthSlider != null) //������ڶ�ӦUGUI��Ѫ��
			healthSlider.value = health * 100;//�޸Ķ�ӦUGUI��Ѫ��
	}
}
