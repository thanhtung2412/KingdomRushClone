using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int dame; 
    public int armor;
    public float moveSpeed;
    public float maxHeal;
    public float currentHeal;
    public int timerespawn; 
    private Transform slider;
    public float coldown;

    public GameObject target = null; 
    private Animator _anim;
    public Vector3 positionFlag { get; set; } 
    public List<GameObject> ListEnemy = new List<GameObject>();
    public GameObject _click;
    public float radius;
    public bool isDead = false;
    public EnemyController _target = null;
    public bool isAttacking = false;
    public bool allowMove = true;
    private float timeregend = 0;
    public bool MustReturn = false; 
    private EnemyController _enemycode;
    private bool waclick = false;
    private GameObject click = null;
    private SpriteRenderer _sprite;
    private Canvas _canvas;
    private bool isShooting = false;
    public bool isRange;
    public Transform Pos_arrow;
    void Start()
    {
        _canvas = GameObject.Find("Canvas_Info").GetComponent<Canvas>();
        _sprite = GetComponent<SpriteRenderer>();
        positionFlag = transform.position;
        currentHeal = maxHeal;
        if (!isRange)
        {
            InvokeRepeating("UpdateTarget", 0f, 0.2f);
        }
        else
        {
            InvokeRepeating("UpdateTarget2", 0f, 0.2f);
        }

        slider = transform.Find("HealBar").transform.Find("slider");
        _anim = GetComponent<Animator>();
    }
    private void Regend() 
    {
        timeregend += Time.deltaTime;
        if (timeregend >= 3)
        {
            if (currentHeal > 0 && currentHeal < maxHeal)
            {
                currentHeal += Time.deltaTime * 4;
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1);
            }
            else if (currentHeal >= maxHeal)
            {
                currentHeal = maxHeal;
                slider.transform.localScale = new Vector3(1, 1, 1);
                timeregend = 0f;
            }
        }
    }
    void UpdateTarget() 
    {
        if (target == null || allowMove == true) MustReturn = false;
        if (allowMove == false) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>() != null)
            {
                _enemycode = enemy.GetComponent<EnemyController>();
            }
            if (enemy == null) return;
            float range = Vector2.Distance(transform.position, enemy.transform.position); 
            if (range <= radius && _enemycode.currentHeal > 0)
            {
                if (!ListEnemy.Contains(enemy) && enemy != null) { ListEnemy.Add(enemy); } 

            }
            else 
                ListEnemy.Remove(enemy);
        }
        RemoveNull();
      
        foreach (GameObject enemy in ListEnemy)
        {
            if (enemy == null) return;
            if (enemy.GetComponent<EnemyController>()._KnightToFight == gameObject)
            {
                SetTarget(enemy);
                MustReturn = true;
                return;
            }
        }
        if (MustReturn == true) return;
  
        if (ListEnemy.Count == 1)
        {
            isAttacking = false;
            if (ListEnemy[0] == null) return;
            SetTarget(ListEnemy[0]);
            if (CheckEnemyToFight4Ever(this, ListEnemy[0]))
            {
                MustReturn = true;
            }
            else if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && ListEnemy[0].GetComponent<EnemyController>()._KnightToFight == null)
            {
                isAttacking = false;
                ListEnemy[0].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
            }
        }
        if (MustReturn == true) return;
        else if (ListEnemy.Count == 2)
        {
            if (ListEnemy[0] == null || ListEnemy[1] == null) return;
            isAttacking = false;
            RemoveNull();
            for (int i = 0; i <= 1; i++)
            {

                if (CheckEnemyToFight4Ever(this, ListEnemy[i]) == true)
                {
                    print("da chon duoc Enemy o vong for thu" + i);
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }
                else if (!CheckEnemyToFight4Ever(this, ListEnemy[i]) && ListEnemy[i].GetComponent<EnemyController>()._KnightToFight == null)
                {
                    if (MustReturn == true) return;
                    ListEnemy[i].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }

            }

            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && target == null)
            {
                SetTarget(ListEnemy[1]);
                MustReturn = true;
            }
        }
        else if (ListEnemy.Count >= 3)
        {
            RemoveNull();
            isAttacking = false;
            if (ListEnemy[0] == null || ListEnemy[1] == null || ListEnemy[2] == null) return;
            foreach (GameObject enemy in ListEnemy)
            {
                // RemoveNull();
                if (CheckEnemyToFight4Ever(this, enemy)) 
                {
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (MustReturn == true) return;
            foreach (GameObject enemy in ListEnemy)
            {
                // RemoveNull();
                if (!CheckEnemyToFight4Ever(this, enemy) && enemy.GetComponent<EnemyController>()._KnightToFight == null)
                {
                    if (MustReturn == true) return;
                    enemy.GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && !CheckEnemyToFight4Ever(this, ListEnemy[2]) && target == null)
            {
                SetTarget(ListEnemy[Random.Range(0, 3)]);
                MustReturn = true;
            }
        }
    }
    private void UpdateTarget2()
    {
        if (isDead == true) return;
        if (target == null || allowMove == true) MustReturn = false;
        if (allowMove == false|| MustReturn==true) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] flies = GameObject.FindGameObjectsWithTag("Fly");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) return;
            float range = Vector2.Distance(transform.position, enemy.transform.position); 
            if (range <= radius)
            {
                if (!ListEnemy.Contains(enemy) && enemy != null) { ListEnemy.Add(enemy); }
            }
            else 
                ListEnemy.Remove(enemy);
        }
        foreach (GameObject enemy in flies)
        {
            if (enemy == null) return;
            float range = Vector2.Distance(transform.position, enemy.transform.position); 
            if (range <= radius)
            {
                if (!ListEnemy.Contains(enemy) && enemy != null)
                { ListEnemy.Add(enemy); } 
            }
            else 
                ListEnemy.Remove(enemy);
        }
        RemoveNull();
        if (ListEnemy.Count > 0)
        {
            target = ListEnemy[0];
        }
    }
    private void SetTarget(GameObject enemy) 
    {
        target = enemy;
        _target = target.GetComponent<EnemyController>();
    }
    private bool CheckEnemyToFight4Ever(Hero knight, GameObject enemy)
    {
        GameObject KnightToFight = enemy.GetComponent<EnemyController>()._KnightToFight;
        if (KnightToFight == knight.gameObject) 
        {
            return true;
        }
        else
            return false;
    }
    private void setISAttackingfalse()
    {
        isAttacking = false;
        _anim.SetInteger("Change", 0);
    }
    private void SetIsShootingFalse()
    {
        isShooting = false;
        _anim.SetInteger("Change", 0);
    }
    public void CheckFlipWith(GameObject enemy) 
    {
        if (transform.position.x <= enemy.transform.position.x)
        {
            if (transform.localScale.x == -1)
            {
                transform.localScale = new Vector3(1, 1, 1);
                slider.parent.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            }
        }
        else
        {
            if (transform.localScale.x == 1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                slider.parent.transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
            }
        }
    }
    void Update()
    {
        if (Manager.isFinishing == true) { _anim.enabled = false; return; }
        // đặt layer theo vị trí tọa độ Y
        int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 20)), 1, 150);
        _sprite.sortingOrder = layer;

        if (isDead) return;
        if (waclick == true && Input.GetMouseButtonUp(0)) 
        {
            waclick = false;
            if (click == null)
                click = Instantiate(_click) as GameObject;
        }
        if (target == null) 
        {
            isAttacking = false;
            if (gameObject.transform.position == positionFlag) 
            {
                if (!isAttacking)
                {
                    _anim.SetInteger("Change", 0);
                }
                RegendHeal();
                allowMove = true;
            }
            else
            {
                if (transform.position.x <= positionFlag.x)
                {
                    if (transform.localScale.x == -1)
                    {
                        transform.localScale = new Vector3(1, 1, 1);
                        slider.parent.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                    }
                }
                else
                {
                    if (transform.localScale.x == 1)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                        slider.parent.transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
                    }
                }
                transform.position = Vector2.MoveTowards(transform.position, positionFlag, Time.deltaTime * moveSpeed);
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Run")) ;
                _anim.SetInteger("Change", 1);
            }
        }
        else 
        {
            if (gameObject.transform.position == positionFlag) 
            {
                allowMove = true;
            }
            else
            {
                if (allowMove == false)
                {
                    if (transform.position.x <= positionFlag.x)
                    {
                        if (transform.localScale.x == -1)
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                            slider.parent.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                        }
                    }
                    else
                    {
                        if (transform.localScale.x == 1)
                        {
                            transform.localScale = new Vector3(-1, 1, 1);
                            slider.parent.transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
                        }
                    }
                    transform.position = Vector2.MoveTowards(transform.position, positionFlag, Time.deltaTime * moveSpeed);
                    _anim.SetInteger("Change", 1);
                }
            }

            if (!isRange)
            {
                if (Vector2.Distance(transform.position, target.transform.position) > 1f) 
                {
                    target = null;
                    return;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, target.transform.position) > 1.2f) 
                {
                    target = null;
                    return;
                }
            }
            CheckFlipWith(target);
                                  
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (isRange && distance > 0.3f)
            {
                if (isShooting == false)
                {
                    isShooting = true;
                    CheckFlipWith(target);
                    Invoke("SetIsShootingFalse", coldown);
                    Shoot();
                }
                return;
            }
            else if (isRange && distance <= 0.3f)
            {
                if (target.GetComponent<FlyController>() != null)
                {
                    return;
                }
                else if (target.GetComponent<EnemyController>() != null)
                {
                    SetTarget(target);
                    if (CheckEnemyToFight4Ever(this, target) == false)
                    {
                        target.GetComponent<EnemyController>()._KnightToFight = gameObject;
                    }
                }
            }
            if (transform.localScale.x == 1)
            {
                if (transform.position != target.transform.position + new Vector3(-0.2f, 0, 0) && allowMove) 
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position + new Vector3(-0.2f, 0, 0), Time.deltaTime * moveSpeed);
                    _anim.SetInteger("Change", 1);
                    if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 0.2f && Mathf.Abs(transform.position.y - target.transform.position.y) <= 0.1f)
                    {
                        AttackEnemy();
                    }
                }
                else
                {
                    AttackEnemy();
                }
            }
            else if (transform.localScale.x == -1)
            {
                if (transform.position != target.transform.position + new Vector3(0.2f, 0, 0) && allowMove)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position + new Vector3(0.2f, 0, 0), Time.deltaTime * moveSpeed);
                    _anim.SetInteger("Change", 1);
                    if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 0.2f && Mathf.Abs(transform.position.y - target.transform.position.y) <= 0.1f)
                    {
                        AttackEnemy();
                    }
                }
                else
                {
                    AttackEnemy();
                }
            }
        }


    }
    private void Shoot()
    {
        int _random = Random.Range(0, 7);
        if (_random == 3 || _random == 2)
        {
            _anim.SetInteger("Change", 4);
        }
        else if (_random == 5)
        {
            if (currentHeal < maxHeal)
            {
                _anim.SetInteger("Change", 5);
                currentHeal += 50;
                if (currentHeal >= maxHeal) { currentHeal = maxHeal; }
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);
            }
        }
        else
            _anim.SetInteger("Change", 2);

    }
    private void InstanceArrow()
    {
        SoundTower._instance.ShootArrow();
        GameObject arrow = Instantiate(Resources.Load("Archer/Arrow"), Pos_arrow.position, Quaternion.identity) as GameObject;
        Parabol _arrow = arrow.GetComponent<Parabol>();
        if (target != null)
        {
            _arrow.target = target;
            _arrow.Dame = dame;
        }
    }
    private void InstacnceGreenArrow()
    {
        SoundTower._instance.ShootArrow();
        GameObject arrow = Instantiate(Resources.Load("Archer/GreenArrow"), Pos_arrow.position, Quaternion.identity) as GameObject;
        Parabol _arrow = arrow.GetComponent<Parabol>();
        if (target != null)
        {
            _arrow.target = target;
            _arrow.Dame = dame;
        }
        else
        {
            Destroy(arrow);
        }
        if (ListEnemy.Count > 2)
        {
            GameObject arrow2 = Instantiate(Resources.Load("Archer/GreenArrow"), Pos_arrow.position, Quaternion.identity) as GameObject;
            Parabol _arrow2 = arrow2.GetComponent<Parabol>();
            if(ListEnemy[1] != null)
            {
                _arrow2.target = ListEnemy[1];
                _arrow2.Dame = dame;
            }
            else
            {
                Destroy(arrow2);
            }

        }
    }
    private void AttackEnemy() 
    {
        if (isAttacking == false)
        {
            _anim.StopRecording();
            SoundTower._instance.KnightFight();
            isAttacking = true;
            Invoke("setISAttackingfalse", coldown);
            if (_target.StartAttack == false)
            {
                _target.StartAttack = true;
            }
            int _random = Random.Range(0, 11);
            if (_random == 9) 
            {
                if (currentHeal < maxHeal)
                {
                    _anim.SetInteger("Change", 5);
                    currentHeal += 50;
                    if (currentHeal >= maxHeal) { currentHeal = maxHeal; }
                    slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);
                }
            }
            else if (_random == 4 || _random == 5 || _random == 6) 
            {
                if (!isRange)
                    _anim.SetInteger("Change", 2);
                else
                    _anim.SetInteger("Change", 3);
            }
            else 
            {
                _anim.SetInteger("Change", 3);
            }
        }
    }
    public void GiveDame()
    {
        if (_target != null)
        {
            _target.TakeDamePhysic(dame);
            if (_target.currentHeal <= 0)
            {
                target = null;
                _target = null;
            }
        }
    }
    public void TakeDame(int _dame) 
    {
        if (currentHeal > 0)
        {
            if (!isRange)
            {
                int _ran = Random.Range(0, 10);
                if (_ran != 5) 
                {
                    int trueDame = _dame - armor;
                    currentHeal -= trueDame;
                    if (currentHeal > 0)
                        slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);
                    else
                        Die();
                }
                else
                {
                    isAttacking = true;
                    Invoke("setISAttackingfalse", 1f);
                    _anim.SetInteger("Change", 4);
                }
            }
            else
            {
                int trueDame = _dame - armor;
                currentHeal -= trueDame;
                if (currentHeal > 0)
                    slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);
                else
                    Die();
            }
        }
        else
            Die();
    }
    private void RegendHeal() 
    {
        timeregend += Time.deltaTime;
        if (timeregend >= 3)
        {
            if (currentHeal > 0 && currentHeal < maxHeal)
            {
                currentHeal += Time.deltaTime * 4;
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1);
            }
            else if (currentHeal >= maxHeal)
            {
                currentHeal = maxHeal;
                slider.transform.localScale = new Vector3(1, 1, 1);
                timeregend = 0f;
            }
        }
    }
    private void Die() 
    {
        if (isDead == true) return;
        _anim.SetInteger("Change", 6);
        isDead = true;
        SpawnLevel._instace.InstanceHero(timerespawn);
        Destroy(gameObject, 1f);
    }
    private void RemoveNull() 
    {
        for (int i = 0; i < ListEnemy.Count; i++)
        {
            if (ListEnemy[i] == null)
                ListEnemy.Remove(ListEnemy[i]);
        }
    }
    private void OnMouseDown()
    {
        _canvas.enabled = true;
        if(!isRange)
        _canvas.transform.GetChild(4).gameObject.SetActive(true);
        else
            _canvas.transform.GetChild(5).gameObject.SetActive(true);
        waclick = true;
    }   
}
