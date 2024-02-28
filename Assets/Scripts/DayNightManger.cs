using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayNightManger : MonoBehaviour
{
    private Light directionalLight;
    [SerializeField,Range(0f, 24f)] private float timeOfDay;

    public bool isNight = false;
    private string keyIsNight = " isNight";
    public bool AutoChange = false;//자동 낮밤 변경

    [SerializeField,Range(0f,24f)] float dayTime = 14;
    [SerializeField,Range(0f,24f)] float nightTime = 23;

    List<FunctionLight> listhLights = new List<FunctionLight>();

    private void OnApplicationQuit()
    {
        string value = isNight == true ? "t" : "f";
        PlayerPrefs.SetString(keyIsNight, value);
    }

    private void Awake()
    {
        string value = PlayerPrefs.GetString(keyIsNight, "f");
        isNight = value == "t" ? true : false;
        if(directionalLight == null)
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            int count = lights.Length;
            for(int iNum =0; iNum < count; ++iNum)
            {
                Light light = lights[iNum];
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                }
                else
                {
                    listhLights.Add(light.transform.parent.GetComponent<FunctionLight>());
                }
            }
            count = listhLights.Count;
            for(int iNum = 0; iNum < count; ++iNum)
            {
                FunctionLight light = listhLights[iNum];
                light.init(isNight);
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        timeOfDay %= 24;//0 ~ 24 사이의 값만 저장
        if(AutoChange == true)
        {
            //timeOfDay += Time.deltaTime;//시간이 자동으로 증가
            if (timeOfDay > dayTime)//14
            {
                isNight = true;
            }
            else if(timeOfDay > nightTime)//23
            {
                isNight = false;
            }
        }

        else//시간을 직접 관리
        {
            if(isNight == true)
            {
                timeOfDay += Time.deltaTime;
                if(timeOfDay > nightTime)
                {
                    timeOfDay = nightTime;//밤
                }
            }
            else
            {
                timeOfDay += Time.deltaTime;
                if(timeOfDay < nightTime && timeOfDay > dayTime)
                {
                    timeOfDay = dayTime;//낮
                }
            }
        }
        if (timeOfDay > 23.1)
        {
            timeOfDay = 4;
        }
        updateLighting();
    }

    private void updateLighting()
    {
        if (directionalLight == null)
        {
            Debug.LogError("게임에 디렉셔널 라이트가 존재하지 않습니다.");
            return;
        }
        float timePercent = timeOfDay / 24f;//0 ~ 1
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90f, 150f, 0));
        if(directionalLight.transform.eulerAngles.x >= 180f)//밤
        {
            if(directionalLight.color.r > 0.0f)
            {
                directionalLight.color -= new Color(1, 1, 1) * Time.deltaTime * 10.0f;
                if(directionalLight.color.r < 0.0f)
                {
                    directionalLight.color = new Color(0, 0, 0);

                    foreach (FunctionLight light in listhLights)
                    {
                        light.TurnOnLight(true);
                    }
                }
            }
        }
        else if(directionalLight.transform.localRotation.x >= 0.0f)//낮
        {
            if(directionalLight.color.r < 1.0f)
            {
                directionalLight.color += new Color(1, 1, 1) * Time.deltaTime * 10.0f;
                if(directionalLight.color.r > 1.0f)
                {
                    directionalLight.color = new Color(1, 1, 1);

                    foreach (FunctionLight light in listhLights)
                    {
                        light.TurnOnLight(false);
                    }
                }
            }
        }
    }
}
