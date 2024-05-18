using UnityEngine;
using UnityEngine.AI;

public class playerMove : MonoBehaviour
{
    public enum Movement { Transform, Agent }
    public Movement currentMovement;

    public NavMeshAgent agent;

    public GameObject model;
    public Vector3 positionForShot;
    public Animator animator;
    float speed = 5;

    [SerializeField] GameObject viewPoint;
    [SerializeField] LookAt camerasc;

    private GameManager gameManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();
        currentMovement = Movement.Transform;
        agent.enabled = false;
    }

    void Update()
    {

        //Intercambia las acciones del juego segun el estado actual.
        switch (currentMovement)
        {
            case Movement.Transform:
                GameRoom();
                break;
            case Movement.Agent:
                Aget();
                break;

            default:
                break;
        }

    }
    void GameRoom()
    {
        if (Vector3.Distance(new Vector3(positionForShot.x,0,0), new Vector3(transform.position.x,0,0)) > 0.1f 
            && gameManager.allShoot)
        {
            if(transform.position.x > positionForShot.x)
            {
                //Caminar a la izquierda.
                model.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                //animator.SetBool("L", true);
            }
            else
            {
                //Caminar a la dercha
                model.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                //animator.SetBool("R", true);
            }
        }
        else
        {
            //animator.SetBool("L", false);
            //animator.SetBool("R", false);
            model.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    void Aget()
    {
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            viewPoint.transform.position = new Vector3(transform.position.x,
                viewPoint.transform.position.y,
                viewPoint.transform.position.z);
            camerasc.transform.position = new Vector3(transform.position.x, 
                camerasc.transform.position.y, 
                camerasc.transform.position.z);

            camerasc.MoveCamera(transform.position.x);

            //animator.SetBool("R", true);
        }
        else
        {
            //animator.SetBool("R", false);
            
        }
    }

    void SetAgentDestination(Transform target)
    {
        if (target != null)
        {
            agent.enabled = true;
            agent.SetDestination(target.position);
        }
    }

    public void SetPositionForShot(Vector3 target)
    {
        positionForShot = target;
    }

    public void GoShop(Transform pos)
    {
        SetAgentDestination(pos);
        currentMovement = Movement.Agent;
    }

    public void GoGameRoom(Transform pos)
    {
        SetAgentDestination(pos);
        currentMovement = Movement.Agent;
    }

    public void Continue()
    {
        transform.position = new Vector3(positionForShot.x,transform.position.y,transform.position.z);
    }
}
