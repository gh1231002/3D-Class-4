using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputControler : MonoBehaviour
{
    Animator anim;
    [SerializeField]Transform trsLookAt;
    [SerializeField, Range(0.0f, 1.0f)] float LookAtWeight;

    List<string> listDanceSatatName = new List<string>();

    [SerializeField] GameObject objInven;
    [SerializeField] GameObject objButton;

    Dictionary<string, string> dicNameValue = new Dictionary<string, string>();

    [SerializeField, Range(0.0f, 1.0f)] float distanceToGround;

    private void OnAnimatorIK(int layerIndex)
    {
        if(trsLookAt != null)
        {
            anim.SetLookAtWeight(LookAtWeight);
            anim.SetLookAtPosition(trsLookAt.position);
        }

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

        if(Physics.Raycast(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, out RaycastHit lefthit, distanceToGround + 1.0f, LayerMask.GetMask("Ground")))
        {
            Vector3 footPos = lefthit.point;
            footPos.y += distanceToGround;
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);

            anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, lefthit.normal),lefthit.normal));
        }
        if (Physics.Raycast(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down, out RaycastHit righthit, distanceToGround + 1.0f, LayerMask.GetMask("Ground")))
        {
            Vector3 footPos = righthit.point;
            footPos.y += distanceToGround;
            anim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);

            anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, righthit.normal), righthit.normal));
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

        dicNameValue.Add("Dance_1", "어떤춤");
        dicNameValue.Add("Dance_2", "어떤 어떤 춤");
        dicNameValue.Add("Dance_3", "어떤 무엇 춤");
    }

    void Start()
    {
        initDance();
        creatDanceUi();
    }

    void Update()
    {
        moving();
        doDance();
        activeDance();
    }


    private void moving()
    {
        //float 형식으로 이동구현
        anim.SetFloat("SpeedVertical", Input.GetAxis("Vertical"));
        anim.SetFloat("SpeedHorizontal", Input.GetAxis("Horizontal"));

        //bool 형식으로 이동구현
        //anim.SetBool("Front",Input.GetKey(KeyCode.W));
        //anim.SetBool("Back",Input.GetKey(KeyCode.S));
        //anim.SetBool("Right",Input.GetKey(KeyCode.D));
        //anim.SetBool("Left",Input.GetKey(KeyCode.A));
    }

    /// <summary>
    /// 특정키를 입력했을때 댄스인벤토리가 열리거나 닫힘
    /// </summary>
    private void activeDance()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = objInven.activeSelf;
            objInven.gameObject.SetActive(!isActive); //어떤값의 반대로 넣을때는 ! 넣기
        }
    }

    /// <summary>
    /// 어떤 댄스들을 가지고있는지 확인후 리스트에 추가
    /// </summary>
    private void initDance()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        int count = clips.Length;
        for(int iNum = 0; iNum < count; ++iNum)
        {
            string animName = clips[iNum].name;
            if (animName.Contains("Dance_"))//어떤 문자열을 가지고 있는지 체크하고싶을때
            {
                listDanceSatatName.Add(animName);
            }
        }
    }

    /// <summary>
    /// 가지고있는 댄스 수에 맞춰서 Ui를 생성
    /// </summary>
    private void creatDanceUi()
    {
        Transform parent = objInven.transform;
        int count = listDanceSatatName.Count;
        for(int iNum = 0; iNum < count; ++iNum)
        {
            int Number = iNum;

            GameObject fab = Instantiate(objButton,parent);

            TMP_Text objText = fab.GetComponentInChildren<TMP_Text>();

            string curName = listDanceSatatName[iNum];
            objText.text = dicNameValue[curName];

            Button objBtn = fab.GetComponent<Button>();
            objBtn.onClick.AddListener(() =>
            {
                anim.CrossFade(listDanceSatatName[Number], 0.1f);
            });
        }
    }

    private void doDance()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //anim.Play("Dance 1");
            anim.CrossFade("Dance 1", 0.2f);//절대값 아님,0~1
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            //anim.Play("Dance 2");
            anim.CrossFade("Dance 2", 0.2f);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            //anim.Play("Dance 3");
            anim.CrossFade("Dance 3", 0.2f);
        }

        if(Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)//Unity 애니메이터에서 연결해서 해도 가능
        {
            anim.Play("Move");
        }
    }
}
