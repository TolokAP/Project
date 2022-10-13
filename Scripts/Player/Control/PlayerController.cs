
using UnityEngine;
using Photon.Bolt;


public class PlayerController : EntityBehaviour<IPlayer>
{
    private Vector3 moveVector;
    private FixedJoystick joistick;
    private CharacterController ch_controller;
    public float speedMove = 4f;
    public float runSpeed = 8f;
    private GameObject _camera;
    Vector3 offset;
  

  
    public override void Attached()
    {
        
        state.SetTransforms(state.Transform, transform);
        state.SetAnimator(GetComponent<Animator>());
      

        state.Animator.applyRootMotion = entity.IsOwner;
        ch_controller = GetComponent<CharacterController>();
        joistick = GameObject.FindGameObjectWithTag("Joy").GetComponent<FixedJoystick>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        offset = new Vector3(0, 30, -24);
       
    }

    
  public void FootR()
    {
        Debug.Log("Правый шаг");
    }
    public void FootL()
    {
        Debug.Log("Левый шаг");
    }

    private void Update()
    {
        if (entity.IsOwner)
        {
            if (!state.Dead && !state.Stunned)
            {
                CharacterMove();

                //Get local velocity of charcter
                float velocityZel;

                velocityZel = Mathf.Sqrt(joistick.Horizontal * joistick.Horizontal + joistick.Vertical * joistick.Vertical);

                ////Update animator with movement values

                state.VelocityZ = velocityZel;

                if (moveVector.x != 0 || moveVector.z != 0)
                {
                    state.Moving = true;
                    state.Attack = false;

                }
                else
                {
                    state.Moving = false;

                }

            }
        }

    }
    void LateUpdate()
    {

        if (entity.IsOwner)
        {
            CameraTransform();

        }

    }

    private void CameraTransform()
    {
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, transform.position + offset, 4f * Time.deltaTime);
    }
    private void CharacterMove()
    {
        moveVector = Vector3.zero;



        moveVector.x = joistick.Horizontal * speedMove;
        moveVector.z = joistick.Vertical * speedMove;


        if (moveVector.x != 0 || moveVector.z != 0)
        {
            state.Moving = true;

        }
        else
        {
            state.Moving = false;

        }

        if (Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, moveVector, speedMove, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }



        ch_controller.Move(moveVector * Time.deltaTime);

    }

    
}
