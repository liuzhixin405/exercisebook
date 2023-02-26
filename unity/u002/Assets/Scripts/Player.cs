using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody rd;
    public int score = 0;
    public Text scoreText;
    public GameObject winText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("������");
        rd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("������");
        //rd.AddForce(Vector3.left);
        float h=Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        rd.AddForce(new Vector3(h,0,v)*10);
        //Debug.Log(h);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("��������ײ");
        ////var tag = collision.gameObject.tag;
        //if (collision.gameObject.tag == "Food")
        //{
        //    Destroy(collision.gameObject);
        //}
    }
    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("��ײ���뿪��");
    //}
    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("��ײ�Ӵ���");
    //}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTriggerEnter" + other.tag);
        if (other.tag == "Food")
        {
            Destroy(other.gameObject);
            score++;
            scoreText.text = $"����Ϊ:{score}";
            if(score == 12)
            {
                winText.SetActive(true);
            }
        }
       
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("OnTriggerExit" + other.tag);
    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("OnTriggerStay" + other.tag);
    //}
}
