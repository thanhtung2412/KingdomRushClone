using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{

    [HideInInspector]
    public GameObject CurrentZone, NextZone; 

    public int Index, next_index;
    [HideInInspector]
    public List<GameObject> enemies = new List<GameObject>();
    private GameObject target; 
    public float coldown;
    public int Dame;
    private float searchvalue = 0.5f;
    public Transform pos_bullet; 
    private Animator _anim;
    [HideInInspector]
    public GameObject Interface = null; 
    [Header("Image bullet dùng để Reload sau khi bắn")]
    public GameObject bulletImage; 
    private bool isShooting = false;
    private Canvas _canvas;
    private Text Speedtext, dametext, rangetext;
    void Start()
    {
        #region Khoi tao UI Info
        _canvas = GameObject.Find("Canvas_Info").GetComponent<Canvas>();
        dametext = _canvas.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        Speedtext = _canvas.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        rangetext = _canvas.transform.GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();
        if (_canvas.enabled)
        {
            ShowUI_Infomation();
        }
        #endregion
        #region Load hình ảnh của Zone/ tầm bắn của trụ
        next_index = Index + 1;
        CurrentZone = Instantiate(Resources.Load("Cannon/Zone" + Index)) as GameObject;
        CurrentZone.transform.parent = transform;
        CurrentZone.transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        CurrentZone.GetComponent<SpriteRenderer>().color = Color.blue;
        CurrentZone.SetActive(false);
        if (Resources.Load("Cannon/Zone" + next_index) != null)
        {
            NextZone = Instantiate(Resources.Load("Cannon/Zone" + next_index)) as GameObject;
            NextZone.transform.parent = transform;
            NextZone.transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
            NextZone.SetActive(false);
        }
        #endregion
        _anim = GetComponent<Animator>();
        SoundTower._instance.CannonTower();
        Upgrade_property(); 
        int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 15)), 1, 150);
        GetComponent<SpriteRenderer>().sortingOrder = layer;
    }
    private void Upgrade_property()
    {
        if (PlayerPrefs.GetInt("Cannonupgrade1") == 1)
        {
            Dame += 5;
        }
        if (PlayerPrefs.GetInt("Cannonupgrade2") == 1)
        {
            GameObject zone = transform.Find("Zone").gameObject;
            zone.transform.localScale = new Vector2(zone.transform.localScale.x + 0.1f, zone.transform.localScale.y + 0.1f);
        }
        if (PlayerPrefs.GetInt("Cannonupgrade5") == 1)
        {
            Dame += 10;
        }
    }
    void OnMouseDown() 
    {
        SoundTower._instance.Click();
        #region Level Cannon <3
        if (Index < 3)
        {
            if (Interface == null)
            {
                Interface = Instantiate(Resources.Load("Interface/CannonInterface" + Index), transform.position, Quaternion.identity) as GameObject;
                Interface.transform.parent = this.transform;
                CurrentZone.SetActive(true);
                //---Hien thi UI Info
                ShowUI_Infomation();
            }
            else
            {
                CurrentZone.SetActive(false);
                Destroy(Interface, 0.2f);
                _canvas.enabled = false;
                _canvas.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        #endregion
        #region Level Cannon >=3
        else
        {
            if (Interface == null)
            {
                Interface = Instantiate(Resources.Load("Interface/CannonInterface" + Index), transform.position, Quaternion.identity) as GameObject;
                Interface.transform.parent = this.transform;
                ShowUI_Infomation();
            }
            else
            {
                CurrentZone.SetActive(false);
                if (Interface.activeSelf == false)
                {
                    Interface.SetActive(true);
                    ShowUI_Infomation();
                }
                else
                {
                    Interface.SetActive(false);
                    GameObject.Find("Canvas_Tower").GetComponent<Canvas>().enabled = false;
                    _canvas.enabled = false;
                    _canvas.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
    void Update()
    {
        if (Manager.isFinishing == true) return;
        RemoveNull();
        if (enemies.Count > 0)
        {
            if (isShooting == false)
            {
                target = enemies[0];
                isShooting = true;
                Invoke("SetShootingFalse", coldown);
                Shoot();
            }
        }
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Vector3.zero);
        if (hit.collider == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _canvas.enabled = false;
                _canvas.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        else
        {
            if (hit.collider.gameObject.tag == "LimitSpell")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _canvas.enabled = false;
                    _canvas.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }
    }

    private void Shoot()
    {
        SoundTower._instance.Cannon_Fire();
        _anim.SetTrigger("Fire");
    }
    private void SetShootingFalse()
    {
        isShooting = false;
    }
    public void ShowImageBullet()
    {
        InstanceBullet(pos_bullet);
    }
    private void InstanceBullet(Transform pos)
    {
        GameObject Bullet = Instantiate(Resources.Load("Cannon/Bullet" + Index), pos.transform.position, Quaternion.identity) as GameObject;
        Parabol _Bullet = Bullet.GetComponent<Parabol>();
        _Bullet.Dame = Dame;
        if (enemies.Count > 0)
        {
            if (target == null && enemies[0] != null) target = enemies[0];
            _Bullet.target = target;
            _Bullet.maxLaunch = getminSpeed();
        }
        else
            Destroy(Bullet);
    }
    #region hàm điều chỉnh lực bắn viên đạn
    private float getminSpeed()
    {
        float aux = 3f;
        while (moreSpeed(aux) == true)
        { aux = aux + searchvalue; }
        return aux;
    }
    private bool moreSpeed(float speed)
    {
        if (enemies[0] == null) return false;
        bool aux = false;
        float xTarget = enemies[0].transform.position.x;
        float yTarget = enemies[0].transform.position.y;
        float xCurrent = transform.position.x;
        float yCurrent = transform.position.y;
        float xDistance = Mathf.Abs(xTarget - xCurrent);
        float yDistance = yTarget - yCurrent;
        float fireAngle = 1.57075f - (float)(Mathf.Atan((Mathf.Pow(speed, 2f) + Mathf.Sqrt(Mathf.Pow(speed, 4f) - 9.8f * (9.8f * Mathf.Pow(xDistance, 2f) + 2f * yDistance * Mathf.Pow(speed, 2f)))) / (9.8f * xDistance)));
        float xSpeed = (float)Mathf.Sin(fireAngle) * speed;
        float ySpeed = (float)Mathf.Cos(fireAngle) * speed;
        if ((xTarget - xCurrent) < 0f) { xSpeed = -xSpeed; }
        if (float.IsNaN(xSpeed) || float.IsNaN(ySpeed)) { aux = true; }
        return aux;
    }
    #endregion
    public void AddEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    } 
    public void RemoveEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            enemies.Remove(enemy);
        }
    } 
    private void RemoveNull()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }
    } 
    public void ShowNextZone(bool turn)
    {
        NextZone.SetActive(turn);
    } 
    public void ShowCurrentZone(bool turn)
    {
        CurrentZone.SetActive(turn);
    }
    public void ReloadBullet()
    {
        bulletImage.SetActive(true);
    } 
    private void ShowUI_Infomation()
    {
        _canvas.enabled = true;
        _canvas.transform.GetChild(0).gameObject.SetActive(false);
        _canvas.transform.GetChild(1).gameObject.SetActive(false);
        _canvas.transform.GetChild(3).gameObject.SetActive(false);
        _canvas.transform.GetChild(2).gameObject.SetActive(true);
        dametext.text = Dame.ToString();
        Speedtext.text = "Very Slow";
        if (Index == 0) { rangetext.text = "Average"; }
        else if (Index == 1) { rangetext.text = "Average"; }
        else if (Index == 2) { rangetext.text = "Long"; }
        else if (Index == 3) { rangetext.text = "Long"; }
        else { rangetext.text = "Average"; }
    }
}
