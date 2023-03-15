using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ICanTakeDamage;
using static Initializer;

public class PLayerController : AbstractBehavior
{
    [SerializeField] private Transform groundCheckObject;
    [SerializeField] private Transform frontCheckObject;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask forwardMask;

    private CharacterController controller;    
    private bool isGrounded = true;
    private float smoothVel;
    public Vector3 velocity;
    public static int noOfcliks = 0;
    private float lastClicedTime = 0;
    private float nextFireTime = 0;
    public override void Init()
    {

        if(!controller)controller = GetComponent<CharacterController>();

        List<ItemGrid> buferGrids = new List<ItemGrid>();

        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Шлем).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Броня).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Ремень).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Штаны).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Сапоги).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Оружие).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Щит).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Кольцо).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Кольцо2).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Наплечники).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Ожерелье).GetComponent<ItemGrid>());
        buferGrids.Add(Initializer.singleton.InitObject(InitializerNames.Сетка_Инвентарь).GetComponent<ItemGrid>());

        Chest.InitGrids(buferGrids);
        // chest.InitGrid(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Плеер).GetComponent<ItemGrid>());
        
        // chest.GetChestGrid().chestKeeper = chest;
    }

    private void Update()
    {
        Gravity();

        if (Time.frameCount % 40 == 0)
        {
            ForwardCheckOversphere();
        }

        if(GameManager.singleton.isControlingPlayer)
        {
            Controller();
        }
        else
        {
            MovePlayer(0,0);
        }
    }   

    private void Controller()
    {
        if(!Input.GetKey(KeyCode.Mouse1))
        {
            MovePlayer(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        else
        {
            MovePlayer(0,0);
            RotationPlayer();
        }
        
        if(Time.time - lastClicedTime > 1.5f)
        {
            noOfcliks = 0;
            _animator.SetBool("Attack1", false);
            _animator.SetBool("Attack2", false);
            _animator.SetBool("Attack3", false);
        }


            RotationPlayer();
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {

            lastClicedTime = Time.time;
            
            if(Time.time <= nextFireTime) return;

            noOfcliks ++;

            noOfcliks = Mathf.Clamp(noOfcliks, 0, 3);

            
            if(noOfcliks >= 1)
            {
                _animator.SetBool("Attack1", true);
            }

            if(noOfcliks >= 2)
            {
                _animator.SetBool("Attack2", true);
            }

            if(noOfcliks >= 3)
            {
                _animator.SetBool("Attack3", true);
            }

        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.A))
        {
            _animator.SetBool("уворот влево", true);
        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.D))
        {
            _animator.SetTrigger("уворот вправо");
        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.S))
        {
            _animator.SetTrigger("Уврот назад");
        }

        _animator.SetBool("Block", Input.GetKey(KeyCode.LeftShift));

        if(Input.GetKeyDown(KeyCode.E) && _target != null)
        {
            _target.Use(this);
            _target = null;
        }       
    }
    
    //метод пускает сферу вперед и проверяет что в радиусе по маске
    private void ForwardCheckOversphere()
    {
        if(_target != null)
        {
            try 
            {
                _target.ShowOutline(false);    
            }       
            catch (MissingReferenceException){}
        }

        _target = null;
        
        float distanceToHit = 4;     
        
        frontCheckObject.rotation = Camera.main.transform.rotation;
        
        RaycastHit[] hits = Physics.SphereCastAll(frontCheckObject.transform.position, 0.5f, frontCheckObject.forward, 4, forwardMask, QueryTriggerInteraction.UseGlobal);
        
        if(hits.Length > 0)
        {
            RaycastHit hitObject = hits[0];

            foreach (RaycastHit hit in hits)
            {
                distanceToHit = hit.distance;
                
                Vector3 centr = frontCheckObject.transform.position + frontCheckObject.forward * distanceToHit;

                if(Vector3.Distance(centr, hit.transform.position) < Vector3.Distance(centr, hitObject.transform.position))
                {
                    hitObject = hit;
                }
            }            

            _target = hitObject.transform.GetComponent<ICanUse>();
 
            _target.ShowOutline(true);
        }
    }

    private void MovePlayer(float x, float y)
    {
        _animator.SetFloat("vertical", y, 0.1f, Time.deltaTime);

        _animator.SetFloat("horizontal", x, 0.1f, Time.deltaTime);

        if(y != 0 && GameManager.singleton.cinemachine.m_XAxis.m_MaxSpeed > 0|| x != 0 && GameManager.singleton.cinemachine.m_XAxis.m_MaxSpeed > 0)RotationPlayer();
    }

    //поворачивает плеера в сторону камеры при движении
    private void RotationPlayer()
    {
        float angle = 
        Mathf.SmoothDampAngle(transform.eulerAngles.y, GameManager.singleton.cameraControll.transform.eulerAngles.y, ref smoothVel, 0.1f);

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    private void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheckObject.position,0.4f, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(5 * -1 * -9.18f);

                _animator.SetTrigger("jump");
            }
            
            return; 
        }

        velocity.y += -9.18f * Time.deltaTime; 

        controller.Move(velocity * Time.deltaTime);
    }    
}
