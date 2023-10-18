using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilainBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING }
    public ActionState state = ActionState.IDLE;
    
    public Node.Status treeStatus = Node.Status.RUNNING;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject targetMarker;


    List<GameObject> validItem = new List<GameObject>();


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        tree = new BehaviourTree();
        Sequence collect = new Sequence("Civilain bergerak dan ambil item");

        Leaf mouseInput = new Leaf("Apakah ada input?", CheckInput);
        Leaf moveToTarget = new Leaf("Bergerak ke posisi target",MoveToTarget);
        Leaf checkItem = new Leaf("Apakah ada item di radius NPC?",CheckItem);
        Leaf takeItem = new Leaf("Ambil item", TakeItem);

        collect.AddChild(mouseInput);
        collect.AddChild(moveToTarget);
        collect.AddChild(checkItem);
        collect.AddChild(takeItem);

        tree.AddChild(collect);

        tree.PrintTree();
    }

    public Node.Status CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out RaycastHit hit, float.MaxValue, groundLayer))
            {
                targetPosition = hit.point;
                targetMarker.transform.position = targetPosition;
                return Node.Status.SUCCESS;
            }
        }

        return Node.Status.FAILURE;
    }

    public Node.Status MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(targetPosition, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(targetPosition);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, targetPosition) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    public Node.Status CheckItem()
    {
        Collider[] nearCollider = Physics.OverlapSphere(transform.position, 3f);
        validItem.Clear();

        foreach (Collider collider in nearCollider)
        {
            if(collider.gameObject.tag == "Item")
            {
                validItem.Add(collider.gameObject);
            }
        }

        if (validItem.Count >= 1)
        {
            return Node.Status.SUCCESS;
        }

        return Node.Status.FAILURE;
    }


    public Node.Status TakeItem()
    {
        if(validItem.Count >= 1)
        {
            foreach (GameObject item in validItem)
            {
                item.SetActive(false);
            }

            return Node.Status.SUCCESS;
        }

        return Node.Status.FAILURE;
    }

    private void Update()
    {
        if (treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
    }
}
