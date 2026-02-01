using UnityEngine;

public class Node : MonoBehaviour
{
    public Node nextNode;
    public Node prevNode;

    [SerializeField]
    private GameObject nodeParent;

    private void Start()
    {
        string name = gameObject.name;

        int startIndex = name.IndexOf("(");
        int endIndex = name.IndexOf(")");

        if (startIndex == -1 || endIndex == -1)
        {
            return;
        }

        string numberString = name.Substring(startIndex + 1, endIndex - startIndex - 1);
        int currentNumber = int.Parse(numberString);

        int nextNumber = currentNumber + 1;
        int prevNumber = currentNumber - 1;
        string nextNodeName = $"Node ({nextNumber})";
        string prevNodeName = $"Node ({prevNumber})";

        GameObject foundNextNode = GameObject.Find(nextNodeName);
        GameObject foundPrevNode = GameObject.Find(prevNodeName);

        if (foundNextNode != null)
        {
            nextNode = foundNextNode.GetComponent<Node>();
        }
        else
        {
            GameObject startNode = GameObject.Find("Node (0)");
            if (startNode != null)
            {
                nextNode = startNode.GetComponent<Node>();
            }
        }

        if (foundPrevNode != null)
        {
            prevNode = foundPrevNode.GetComponent<Node>();
        }
        else
        {
            GameObject lastNode = GameObject.Find("Node (" + (nodeParent.transform.childCount - 1).ToString() + ")");
            if (lastNode != null)
            {
                prevNode = lastNode.GetComponent<Node>();
            }
        }
    }
}
