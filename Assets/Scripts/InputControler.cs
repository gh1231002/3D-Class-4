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

    bool doChangeState = false;
    float MouseVertical = 0f;

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

        dicNameValue.Add("Dance_1", "���");
        dicNameValue.Add("Dance_2", "� � ��");
        dicNameValue.Add("Dance_3", "� ���� ��");
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
        checkAim();
    }


    private void moving()
    {
        //float �������� �̵�����
        anim.SetFloat("SpeedVertical", Input.GetAxis("Vertical"));
        anim.SetFloat("SpeedHorizontal", Input.GetAxis("Horizontal"));

        //bool �������� �̵�����
        //anim.SetBool("Front",Input.GetKey(KeyCode.W));
        //anim.SetBool("Back",Input.GetKey(KeyCode.S));
        //anim.SetBool("Right",Input.GetKey(KeyCode.D));
        //anim.SetBool("Left",Input.GetKey(KeyCode.A));
    }

    /// <summary>
    /// Ư��Ű�� �Է������� ���κ��丮�� �����ų� ����
    /// </summary>
    private void activeDance()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = objInven.activeSelf;
            objInven.gameObject.SetActive(!isActive); //����� �ݴ�� �������� ! �ֱ�
        }
    }

    /// <summary>
    /// � ������ �������ִ��� Ȯ���� ����Ʈ�� �߰�
    /// </summary>
    private void initDance()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        int count = clips.Length;
        for(int iNum = 0; iNum < count; ++iNum)
        {
            string animName = clips[iNum].name;
            if (animName.Contains("Dance_"))//� ���ڿ��� ������ �ִ��� üũ�ϰ������
            {
                listDanceSatatName.Add(animName);
            }
        }
    }

    /// <summary>
    /// �������ִ� �� ���� ���缭 Ui�� ����
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
            anim.CrossFade("Dance 1", 0.2f);//���밪 �ƴ�,0~1
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

        if(Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)//Unity �ִϸ����Ϳ��� �����ؼ� �ص� ����
        {
            anim.Play("Move");
        }
    }

    private void checkAim()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && doChangeState == false)
        {
            if(Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;//���콺 Ŀ���� �������ʰ���
                //layer weight�� 1��
                StartCoroutine(changeState(true));
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                //layer weight�� 0����
                StartCoroutine (changeState(false));
            }
        }

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            MouseVertical += Input.GetAxis("Mouse Y") * Time.deltaTime;
            MouseVertical = Mathf.Clamp(MouseVertical, -1f, 1f);
            anim.SetFloat("MouseVertical", MouseVertical);

            if(Input.GetMouseButtonDown(0))
            {
                //anim.Play("���ϴ� �ִϸ��̼� �̸�");
            }
        }
    }

    IEnumerator changeState(bool _upper)
    {
        float timer = 0;
        doChangeState = true;
        if(_upper)
        {
            while(anim.GetLayerWeight(1) < 1.0f)
            {
                timer += Time.deltaTime * 5f;
                anim.SetLayerWeight(1, Mathf.Lerp(0f, 1f, timer));
                yield return null;
            }
        }
        else
        {
            while(anim.GetLayerWeight(1) > 0f)
            {
                timer += Time.deltaTime * 5f;
                anim.SetLayerWeight(1,Mathf.Lerp(1f, 0f, timer));
                yield return null;
            }
        }
        doChangeState = false;
    }
}
