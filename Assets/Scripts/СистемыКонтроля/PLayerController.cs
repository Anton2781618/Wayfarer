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
    private Vector3 velocity;
    public static int noOfcliks = 0;
    private float lastClicedTime = 0;
    private float nextFireTime = 0;
    public override void Init()
    {

        if(!controller)controller = GetComponent<CharacterController>();

        if(!HpSlider) HpSlider = Initializer.singleton.InitObject(InitializerNames.ХПслайдер_Плеера).GetComponent<Slider>();
        if(!MpSlider) MpSlider = Initializer.singleton.InitObject(InitializerNames.МПслайдер_Плеера).GetComponent<Slider>();

        chest.InitChest(Initializer.singleton.InitObject(InitializerNames.Инвентарь_Плеер).GetComponent<ItemGrid>(),
                        Initializer.singleton.InitObject(InitializerNames.Спрайт_денег_Плеер).GetComponent<Image>()
        );
    }

    void Update()
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
            anim.SetBool("attack1", false);
            anim.SetBool("attack2", false);
            anim.SetBool("attack3", false);
        }


        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastClicedTime = Time.time;
            
            if(Time.time <= nextFireTime) return;

            noOfcliks ++;

            noOfcliks = Mathf.Clamp(noOfcliks, 0, 3);

            
            if(noOfcliks >= 1)
            {
                anim.SetBool("attack1", true);
            }

            if(noOfcliks >= 2)
            {
                anim.SetBool("attack2", true);
            }

            if(noOfcliks >= 3)
            {
                anim.SetBool("attack3", true);
            }

        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.A))
        {
            anim.SetBool("уворот влево", true);
        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.D))
        {
            anim.SetTrigger("уворот вправо");
        }

        if(Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.S))
        {
            anim.SetTrigger("Уврот назад");
        }

        anim.SetBool("block", Input.GetKey(KeyCode.LeftShift));

        if(Input.GetKeyDown(KeyCode.E) && target != null)
        {
            target.Use();
            target = null;
        }       
    }

    //метод пускает сферу вперед и проверяет что в радиусе по маске
    private void ForwardCheckOversphere()
    {   
        float curDist = 4;     
        
        frontCheckObject.rotation = Camera.main.transform.rotation;
        
        RaycastHit[] hits = Physics.SphereCastAll(frontCheckObject.transform.position, 0.5f, frontCheckObject.forward, 4, forwardMask, QueryTriggerInteraction.UseGlobal);
        if(hits.Length > 0)
        {
            RaycastHit bufHit = hits[0];

            foreach (RaycastHit hit in hits)
            {
                curDist = hit.distance;
                Vector3 centr = frontCheckObject.transform.position + frontCheckObject.forward * curDist;

                if(Vector3.Distance(centr, hit.transform.position) < Vector3.Distance(centr, bufHit.transform.position))
                {
                    bufHit = hit;
                }
            }            
            
            if(target != null) target.ShowOutline(false);
            target = bufHit.transform.GetComponent<ICanUse>();
            target.ShowOutline(true);
        }
        else
        {
            if(target != null) target.ShowOutline(false);
            target = null;
        }
    }
   
    private void MovePlayer(float x, float y)
    {
        anim.SetFloat("vertical", y, 0.1f, Time.deltaTime);
        anim.SetFloat("horizontal", x, 0.1f, Time.deltaTime);

        if(y != 0 && GameManager.singleton.cameraControll.enabled|| x != 0 && GameManager.singleton.cameraControll.enabled)RotationPlayer();
    }
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
                velocity.y = Mathf.Sqrt(1.6f * -1 * -9.18f);
                anim.SetTrigger("jump");
            }
        }

        velocity.y += -9.18f * Time.deltaTime; 
        controller.Move(velocity * Time.deltaTime);
    }    
}
