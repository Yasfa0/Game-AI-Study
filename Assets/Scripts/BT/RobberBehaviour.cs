using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    NavMeshAgent agent;
    [SerializeField] GameObject diamond;
    [SerializeField] GameObject van;
    [SerializeField] GameObject backDoor;
    [SerializeField] GameObject frontDoor;

    [Range(0, 1000)] public float money = 800;

    public enum ActionState { IDLE,WORKING}
    public ActionState state = ActionState.IDLE;

    public Node.Status treeStatus = Node.Status.RUNNING;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tree = new BehaviourTree();
        Sequence steal = new Sequence("Pencuri Beraksi");
        Leaf toBackDoor = new Leaf("Masuk lewat pintu", GoToBackDoor);
        Leaf toFrontDoor = new Leaf("Masuk lewat pintu", GoToFrontDoor);
        Leaf toDiamond = new Leaf("Nyuri Diamond", GoToDiamond);
        Leaf toVan = new Leaf("Balik lagi ke Van", GoToVan);
        Leaf gotMoney = new Leaf("Apakah punya uang?",CheckMoney);

        Selector chooseDoor = new Selector("Pilih pintu");
        chooseDoor.AddChild(toFrontDoor);
        chooseDoor.AddChild(toBackDoor);

        steal.AddChild(gotMoney);
        steal.AddChild(chooseDoor);
        //steal.AddChild(toBackDoor);
        steal.AddChild(toDiamond);
        //steal.AddChild(toBackDoor);
        steal.AddChild(toVan);

        tree.AddChild(steal);

        tree.PrintTree();
    }

    public Node.Status GoToDiamond()
    {
        Node.Status s = GoToDestination(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!diamond.GetComponent<PickableObject>().isPickedUp)
            {
                //diamond.GetComponent<PickableObject>().PickUpObject();
                diamond.transform.parent = gameObject.transform;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
        {
            return s;
        }
    }

    public Node.Status GoToVan()
    {
        Node.Status s = GoToDestination(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
                money += 300;
                diamond.SetActive(false);
        }
            return s;
        
    }

    public Node.Status CheckMoney()
    {
        if (money >= 500)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }

    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToDestination(door.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
        {
            return s;
        }

    }

    public Node.Status GoToDestination(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination,transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }else if (Vector3.Distance(agent.pathEndPosition,destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    private void Update()
    {
        if(treeStatus != Node.Status.SUCCESS)
           treeStatus = tree.Process();
    }
}
