using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : MonoBehaviour
{
    public GameObject Knight; 
    public Transform Flag;
    private GameObject knight1, knight2, knight3;
    private KnightController _k1, _k2, _k3;
    private Vector3 offset1, offset2, offset3;    
    public float TimeToMove;
    public Transform gate;

    private Animator barrack_Anim;
    public int index = 0;

    [HideInInspector]
    public Vector3 pos1, pos2, pos3; 
    public bool spawnfirsttime = true;
    public Sprite[] _sprites_solider; 
    private GameObject BarrackInter = null; 
    private Canvas _canvas;
    private Image _Image_solider;
    private Text Heal, Dame, armor, respawn;
    void Start()
    {
        #region cho Interface
        _canvas = GameObject.Find("Canvas_Info").GetComponent<Canvas>();
		_Image_solider = _canvas.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();        
        Heal = _canvas.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        Dame= _canvas.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();
        armor = _canvas.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();
        respawn = _canvas.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();
		_Image_solider.sprite = _sprites_solider[index]; // Avata của lính
        #endregion
        #region DO dich vector cho vi tri cua cac Knight
        offset1 = new Vector3(-0.15f, 0.1f, 0);
        offset2 = new Vector3(0, -0.1f, 0);
        offset3 = new Vector3(0.15f, 0.1f, 0);
        #endregion
        Flag = this.transform.gameObject.transform.Find("Flag");
        InvokeRepeating("UpdateLate", 0, 0.5f);
        barrack_Anim = GetComponent<Animator>();
		if (spawnfirsttime) 
		{ StartCoroutine(SpawnFirstTime(1f, 1f, 1f)); }
		else 
            SpawnKnight();
		
		int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 30)), 1, 150); 
        GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    void UpdateLate()
    {
        SetPosFlag();
        if (_k1 != null) 
        {
            Heal.text = _k1.maxHeal.ToString();
            if (_k1.armor < 5) { armor.text = "None"; }
            else if (_k1.armor >= 5 && _k1.armor < 10) { armor.text = "Low"; }
            else { armor.text = "Medium"; }
            Dame.text = _k1.dame.ToString();
            respawn.text = _k1.timerespawn.ToString();
        }
        if(knight1==null||knight2==null||knight3 == null)
        {
            StartRespawn(10);
        }
    }
    void Update()
    {
        if (Manager.isFinishing == true) return;
        #region DI chuyển Knight về phía Flag
        if (_k1 != null)
        {
            if (knight1.activeSelf && (_k1.target == null || _k1.allowMove==false)) // Nếu Knight được sinh ra và được phép đk thì di chuyển nó về phía Flag
            {
				if (knight1.transform.position != (Flag.transform.position + offset1)) // Nếu ko ở vị trí của Flag thì sẽ di chuyển đến đó
                {
					knight1.transform.position = Vector2.MoveTowards(knight1.transform.position, Flag.transform.position + offset1, Time.deltaTime * TimeToMove);
                    if (_k1 != null)
                    {
						_k1._anim.SetInteger("Change", 1); // chạy Animation Run
						_k1.CheckFlipWith(Flag.gameObject); // check hướng quay mặt với mục tiêu là cờ Flag
                    }
                }
            }
        }
        if (_k2 != null)
        {
            if (knight2.activeSelf && (_k2.target == null || _k2.allowMove == false))
            {
                if (knight2.transform.position != (Flag.transform.position + offset2))
                {
                    knight2.transform.position = Vector3.MoveTowards(knight2.transform.position, Flag.transform.position + offset2, Time.deltaTime * TimeToMove);
                    if (_k2 != null)
                    {
                        _k2._anim.SetInteger("Change", 1);
                        _k2.CheckFlipWith(Flag.gameObject);
                    }
                }
            }
        }
        if (_k3 != null)
        {
            if (knight3.activeSelf && (_k3.target == null || _k1.allowMove == false))
            {
                if (knight3.transform.position != (Flag.transform.position + offset3))
                {
                    knight3.transform.position = Vector3.MoveTowards(knight3.transform.position, Flag.transform.position + offset3, Time.deltaTime * TimeToMove);
                    if (_k3 != null)
                    {
                        _k3._anim.SetInteger("Change", 1);
                        _k3.CheckFlipWith(Flag.gameObject);
                    }
                }
            }
        }
        #endregion
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Vector3.zero);
        if (hit.collider == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _canvas.enabled = false;
                _canvas.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            if (hit.collider.gameObject.tag == "LimitSpell")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _canvas.enabled = false;
                    _canvas.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }

    }
    private void SetPosFlag()
    {
        if (_k1 != null)
            _k1.positionFlag = Flag.transform.position + offset1;
        if (_k2 != null)
            _k2.positionFlag = Flag.transform.position + offset2;
        if (_k3 != null)
            _k3.positionFlag = Flag.transform.position + offset3;
    }
    IEnumerator SpawnFirstTime(float time1, float time2, float time3)
    {
        yield return new WaitForSeconds(time1);
        knight1 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
        barrack_Anim.SetTrigger("Open");
        SoundTower._instance.OpenGate();
        knight1.name = "Knight1";
        knight1.transform.parent = this.transform;        
        _k1 = knight1.GetComponent<KnightController>();

        yield return new WaitForSeconds(time2);
        knight2 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
        barrack_Anim.SetTrigger("Open");
        SoundTower._instance.OpenGate();
        knight2.name = "Knight2";
        knight2.transform.parent = this.transform;
        _k2 = knight2.GetComponent<KnightController>();

        yield return new WaitForSeconds(time3);
        knight3 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
        barrack_Anim.SetTrigger("Open");
        SoundTower._instance.OpenGate();
        knight3.name = "Knight3";
        knight3.transform.parent = this.transform;
        _k3 = knight3.GetComponent<KnightController>();
    }
    public void GetinforKnight()
    {
        if (knight1 != null && transform.Find("Knight1"))
        {
            pos1 = knight1.transform.position;
        }
        if (knight2 != null && transform.Find("Knight2"))
        {
            pos2 = knight2.transform.position;
        }
        if (knight3 != null && transform.Find("Knight3"))
        {
            pos3 = knight3.transform.position;
        }
    }
    public void SpawnKnight()
    {
        if (pos1 != Vector3.zero)
        {
            knight1 = Instantiate(Knight, pos1, Quaternion.identity) as GameObject;
            knight1.name = "Knight1";
            knight1.transform.parent = this.transform;
            _k1 = knight1.GetComponent<KnightController>();
        }
        if (pos2 != Vector3.zero)
        {
            knight2 = Instantiate(Knight, pos2, Quaternion.identity) as GameObject;
            knight2.name = "Knight2";
            knight2.transform.parent = this.transform;
            _k2 = knight2.GetComponent<KnightController>();
        }
        if (pos3 != Vector3.zero)
        {
            knight3 = Instantiate(Knight, pos3, Quaternion.identity) as GameObject;
            knight3.name = "Knight3";
            knight3.transform.parent = this.transform;
            _k3 = knight3.GetComponent<KnightController>();
        }
        else
            RespawnKnight();
    }
    public void StartRespawn(int time)
    {
        Invoke("RespawnKnight", time);
    }
    void RespawnKnight()
    {
        if (knight1 == null && !transform.Find("Knight1"))
        {
            knight1 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
            barrack_Anim.SetTrigger("Open");
            SoundTower._instance.OpenGate();
            knight1.name = "Knight1";
            knight1.transform.parent = this.transform;
            _k1 = knight1.GetComponent<KnightController>();
        }
        else if (knight2 == null && !transform.Find("Knight2"))
        {
            knight2 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
            barrack_Anim.SetTrigger("Open");
            SoundTower._instance.OpenGate();
            knight2.name = "Knight2";
            knight2.transform.parent = this.transform;
            _k2 = knight2.GetComponent<KnightController>();
        }
      else  if (knight3 == null && !transform.Find("Knight3"))
        {
            knight3 = Instantiate(Knight, gate.position, Quaternion.identity) as GameObject;
            barrack_Anim.SetTrigger("Open");
            SoundTower._instance.OpenGate();
            knight3.name = "Knight3";
            knight3.transform.parent = this.transform;
            _k3 = knight3.GetComponent<KnightController>();
        }

        Heal.text = _k1.maxHeal.ToString();
        if (_k1.armor < 5) { armor.text = "None"; }
        else if (_k1.armor >= 5 && _k1.armor < 10) { armor.text = "Low"; }
        else { armor.text = "Medium"; }
        Dame.text = _k1.dame.ToString();
        respawn.text = _k1.timerespawn.ToString();
    }
	void OnMouseDown()
    {
        SoundTower._instance.Click();
        if (index < 3)
        {
            if (BarrackInter == null)
            {
                GameObject BarrackInterface = Resources.Load("Interface/BarrackInterface" + index) as GameObject;
                BarrackInter = Instantiate(BarrackInterface, transform.position, Quaternion.identity);
                BarrackInter.transform.parent = transform;
                ShowUI_info();
            }
            else
            {
                _canvas.enabled = false;
                _canvas.transform.GetChild(1).gameObject.SetActive(false);
                Destroy(BarrackInter, 0.1f);
            }
        }
        else
        {
            if (BarrackInter == null)
            {
                BarrackInter = Instantiate(Resources.Load("Interface/BarrackInterface" + index), transform.position, Quaternion.identity) as GameObject;
                BarrackInter.transform.parent = this.transform;
                ShowUI_info();
            }
            else
            {
                if (BarrackInter.activeSelf == false)
                {
                    BarrackInter.SetActive(true);
                    ShowUI_info();
                }
                else
                {
                    BarrackInter.SetActive(false);
                    GameObject.Find("Canvas_Tower").GetComponent<Canvas>().enabled = false;
                    _canvas.enabled = false;
                    _canvas.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
	private void ShowUI_info()
    {
        _canvas.enabled = true;
        _canvas.transform.GetChild(0).gameObject.SetActive(false);
        _canvas.transform.GetChild(2).gameObject.SetActive(false);
        _canvas.transform.GetChild(3).gameObject.SetActive(false);
        _canvas.transform.GetChild(1).gameObject.SetActive(true);
    }
}
