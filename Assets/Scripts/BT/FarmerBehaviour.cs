using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FarmerBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    NavMeshAgent agent;
    [SerializeField] Slider hungerSlider;
    [SerializeField] Text nameText;
    [SerializeField] GameObject kerbau;
    [SerializeField] GameObject jerami;

    [Range(0, 1000)] public float hunger = 800;

    public enum ActionState { IDLE, WORKING }
    public ActionState state = ActionState.IDLE;

    public Node.Status treeStatus = Node.Status.RUNNING;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetupHunger()
    {
        hungerSlider.maxValue = 1000;
        hungerSlider.value = hunger;
    }

    public void UpdateHunger()
    {
        hunger = hungerSlider.value;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupHunger();
        tree = new BehaviourTree();
        Sequence feed = new Sequence("Petani memberi makan Kerbau");
        Leaf toKerbau = new Leaf("Petani gerak ke Kerbau", GoToKerbau);
        Leaf toJerami = new Leaf("Menggiring Kerbau ke Jerami", GoToHay);
        Leaf isHungry = new Leaf("Apakah lapar?", CheckHunger);

        feed.AddChild(isHungry);
        feed.AddChild(toKerbau);
        feed.AddChild(toJerami);

        tree.AddChild(feed);

        tree.PrintTree();
    }

    public Node.Status GoToKerbau()
    {
        Node.Status s = GoToDestination(kerbau.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            //if (!kerbau.GetComponent<PickableObject>().isPickedUp)
            //{
                //diamond.GetComponent<PickableObject>().PickUpObject();
                kerbau.transform.parent = gameObject.transform;
                return Node.Status.SUCCESS;
            //}
            return Node.Status.FAILURE;
        }
        else
        {
            return s;
        }
    }

    public Node.Status GoToHay()
    {
        Node.Status s = GoToDestination(jerami.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            hunger += 300;
            hungerSlider.value = hunger;
            //kerbau.SetActive(false);
        }
        return s;

    }

    public Node.Status CheckHunger()
    {
        if (hunger >= 500)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDestination(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
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

    private void Update()
    {
        if (treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = tree.Process();
            nameText.text = tree.children[tree.currentChild].children[tree.children[tree.currentChild].currentChild].name;
        }
    }
}
