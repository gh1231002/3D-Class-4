using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionLight : MonoBehaviour
{
    public enum TypeLight
    {
        Disable,//����
        Allaways,//�׻�
        OnlyNight,//�㿡��
        OnlyDays,//������
    }

    public TypeLight typeLight;

    Material matWindow;
    GameObject objLight;

    public void init(bool _isNight)
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        matWindow = Instantiate(mr.material);
        mr.material = matWindow;
        objLight = GetComponentInChildren<Light>().gameObject;

        //matWindow.EnableKeyword("_EMISSION");//����
        
        if(_isNight == true && typeLight == TypeLight.OnlyNight)
        {
            TurnOnLight(true);
        }
        else if(_isNight == false && typeLight == TypeLight.OnlyDays)
        {
            TurnOnLight(true);
        }
        else if(typeLight == TypeLight.Allaways)
        {
            TurnOnLight(true);
        }
        else
        {
            TurnOnLight(false);
        }
    }

    public void TurnOnLight(bool _value)
    {
        if(_value == true)
        {
            matWindow.EnableKeyword("_EMISSON");
            objLight.SetActive(true);
        }
        else
        {
            matWindow.DisableKeyword("_EMISSON");//����
            objLight.SetActive(false);
        }
    }
}
