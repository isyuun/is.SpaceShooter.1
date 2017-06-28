using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        //Debug.Log("Click StartButton!!!");
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    public void OnClickOptionBtn(float num)
    {
        Debug.Log("Click OptionButton : " + num);
    }
}