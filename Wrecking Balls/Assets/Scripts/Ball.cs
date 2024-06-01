using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.ParticleSystem;


public class Ball : MonoBehaviour
{
    
    public Vector3 point;
    
    Rigidbody rb;
    public float speed = 10;

    RaycastHit hitInfo;
    
    Vector3 reflectDirection;

    public bool isMoving = false;
    bool catchet = false;

    public Vector3 lastRayPosition;
    public Vector3 lastRayDirection;

    GameManager gameManager;

    public GameObject target;
    AudioControl audioControl;

    float ballPosZ = -0.4f;

    public int numBall;
    public List<Ball> subordinates;

    public bool isSubordinate = false;

    public List<GameObject> targets = new List<GameObject>();

    public GameObject particleS;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        point = transform.position;
        gameManager = FindObjectOfType<GameManager>();
        audioControl = gameManager.audioControl;
        UpdateSpeed();
    }
    private void Update()
    {
        if(gameManager.timeScale != 0 && !isSubordinate)
        {
            CheckDistance();
        }
        foreach (var subordinateBall in subordinates)
        {
            subordinateBall.transform.position = transform.position;
        }

    }

    
    private void FixedUpdate()
    {
        //Mueve la bola a su objetivo
        if (transform.position != point && gameManager.timeScale != 0 && !isSubordinate)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, point, speed * Time.deltaTime));
            
        }
    }

    /// <summary>
    /// Calcula el primer objetivo de la bola.
    /// </summary>
    /// <param name="direction"></param>Direccion en la que la bola debe moverse.
    public void ShootBall(Vector3 direction)
    {
        targets.Clear();
        if (Physics.SphereCast(transform.position, 0.1f, direction, out hitInfo, raycastDistance))
        {
            Vector3 normal = hitInfo.normal;
            reflectDirection = Vector3.Reflect(direction, normal);
            point = transform.position + direction * hitInfo.distance;
            //Mantiene la bola en la misma posision en el eje z.
            point = new Vector3(point.x, point.y, ballPosZ);
            reflectDirection = new Vector3(reflectDirection.x, reflectDirection.y, 0);

            isMoving = true;

            lastRayPosition = transform.position;
            lastRayDirection = direction;

            target = hitInfo.collider.gameObject;
            targets.Add(target);
        }
    }
     
    void CheckDistance()
    {
        //Verifica cuando llega al objetivo.
        if (Vector3.Distance(transform.position, point) <= 0.0001f && isMoving)
        {
            transform.position = point;
            //Comprueba si el objetivo no es nulo.
            if (target != null)
            {
                
                if (target.CompareTag("Cube"))
                {
                    var cubeComponent = target.GetComponent<Cube>();
                    if (cubeComponent.point == 0) return;

                    if (audioControl.isActiveAndEnabled )audioControl.Play();

                    if(subordinates.Count == 0 && !isSubordinate)
                    {
                        cubeComponent.Point(1);//Resta puntos al cubo.
                        Instantiate(particleS, transform.position, Quaternion.identity);
                        NewPoint();//Calcula la nueva position.
                    }
                    else if(subordinates.Count > 0)
                    {
                        if(cubeComponent.point > subordinates.Count)
                        {
                            cubeComponent.Point(subordinates.Count + 1);//Resta puntos al cubo.
                            Instantiate(particleS, transform.position, Quaternion.identity);
                        }
                        else if(cubeComponent.point < subordinates.Count + 1)
                        {
                            int cube = cubeComponent.point;

                            cubeComponent.Point(cubeComponent.point);
                            Instantiate(particleS, transform.position, Quaternion.identity);

                            subordinates[cube - 1].ShootBall(lastRayDirection);
                            subordinates[cube - 1].isSubordinate = false;
                            for (int i = cube; i < subordinates.Count; i++)
                            {
                                subordinates[cube - 1].subordinates.Add(subordinates[i]);
                            }
                            subordinates.RemoveRange(cube - 1, subordinates.Count - (cube - 1));
                        }
                        NewPoint();//Calcula la nueva position.
                    }
                }
                //si el objetivo es un limite de area de juego.
                else if (target.CompareTag("Limit"))
                {
                    NewPoint();//Calcula la nueva position.
                }
                else if (target.CompareTag("Down"))
                {
                    NewPositionShot();
                    foreach (var item in subordinates)
                    {
                        item.NewPositionShot();
                    }
                    subordinates.Clear();
                }
                //Si el objetivo es un potenciador lo recoge y sigue su trayectoria.
                else if (target.CompareTag("PowerUp"))
                {
                    target.GetComponent<PowerUp>().GetPowerUp();
                }
            }
            else
            {
                
                if (gameManager.ballCatches.Count > 0)
                {
                    if(point == gameManager.ballCatches[0].point)
                    {
                        HideBall();
                    }
                    else
                    {
                        if(!catchet) LastRay();
                    }
                }
                else
                {
                    if (!catchet) LastRay();
                }
                

            }
        }
        
    }

    //Lanza un raycast igual al ultimo para comprobar si el objetivo sigue siendo el mismo.
    public void LastRay()
    {
        if (Physics.SphereCast(lastRayPosition,0.1f, lastRayDirection, out hitInfo, raycastDistance))
        {
            Vector3 normal = hitInfo.normal;

            reflectDirection = Vector3.Reflect(lastRayDirection, normal);
            point = lastRayPosition + lastRayDirection * hitInfo.distance;
            //Mantiene la bola en la misma posision en el eje z.
            point = new Vector3(point.x, point.y, ballPosZ);
            reflectDirection = new Vector3(reflectDirection.x, reflectDirection.y, 0);

            target = hitInfo.collider.gameObject;
            targets.Add(target);
        }
    }
    public void NewPoint()
    {
        if (Physics.SphereCast(transform.position, 0.1f, reflectDirection, out hitInfo, raycastDistance))
        {
            lastRayPosition = transform.position;
            lastRayDirection = reflectDirection;

            Vector3 normal = hitInfo.normal;
            reflectDirection = Vector3.Reflect(reflectDirection, normal);
            reflectDirection = new Vector3(reflectDirection.x, reflectDirection.y, 0);
            if (isMoving)
            {
                point = lastRayPosition + lastRayDirection * hitInfo.distance;
                
                //Mantiene la bola en la misma posision en el eje z.
                point = new Vector3(point.x, point.y, ballPosZ);
            }
            target = hitInfo.collider.gameObject;
            targets.Add(target);
        }
    }

    public void NewPositionShot()
    {
        isSubordinate = false;
        
        target = null;
        if (!catchet)
        {
            gameManager.ballCatches.Add(this);
            catchet= true;
        }
        

        //Mueve la bola hacia la nueva posision de lanzamiento cuando la bola vuelve a caer.
        point = new Vector3(gameManager.ballCatches[0].point.x, -3.41f, ballPosZ);
    }

    void HideBall()
    {
        transform.position = point;
        gameManager.ballMoving.Remove(this);
        isMoving = false;
        catchet= false;
        if(gameManager.ballCatches.Count == gameManager.ballList.Count &&
            this == gameManager.ballCatches[gameManager.ballCatches.Count -1])
        {
            gameManager.NewLevel();
        }

        //Desactiva las bolas no visibles para mejorar el rendimiento.
        if (gameManager.ballCatches[0].gameObject == gameObject)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Actualiza la velocidad segun la velocidad del game manager.
    /// </summary>
    public void UpdateSpeed()
    {
        speed = gameManager.speed;
    }
    public float raycastDistance = 20f;
    public float bounceDistance = 5f;


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitInfo.point, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastRayPosition, 0.1f);
        // Set Gizmo color to red for the raycast beam
        Gizmos.color = Color.red;
        // Draw the raycast beam
        Gizmos.DrawLine(transform.position, point);
        // Set Gizmo color to blue for the normal bounce
        Gizmos.color = Color.blue;
        // Draw the normal bounce
        Gizmos.DrawLine(point, reflectDirection + hitInfo.point);

       /* // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, point, out hit, raycastDistance))
        {
            // Draw the raycast beam
            Gizmos.DrawLine(transform.position, point);

            // Set Gizmo color to blue for the normal bounce
            Gizmos.color = Color.blue;

            // Calculate the bounce direction
            Vector3 bounceDirection = Vector3.Reflect(point, hitInfo.normal);

            // Draw the normal bounce
            Gizmos.DrawLine(hitInfo.point, hitInfo.point + bounceDirection * bounceDistance);
        }
        else
        {
            // Draw the raycast beam if it doesn't hit anything
            Gizmos.DrawLine(transform.position, transform.position + point * raycastDistance);
        }*/
    }
}
