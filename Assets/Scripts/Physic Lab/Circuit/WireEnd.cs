using UnityEngine;

public class WireEnd : MonoBehaviour
{
    public Terminal connectedTerminal;
    public Transform snapPoint;
    public float snapDistance = 0.05f;

    private void OnTriggerStay(Collider other)
    {
        Terminal terminal = other.GetComponent<Terminal>();
        if (terminal == null) return;

        if (connectedTerminal == null && !terminal.IsConnected)
        {
            float dist = Vector3.Distance(transform.position, terminal.transform.position);
            if (dist < snapDistance)
            {
                SnapToTerminal(terminal);
            }
        }
    }

    void SnapToTerminal(Terminal terminal)
    {
        connectedTerminal = terminal;
        terminal.connectedWire = this;

        transform.position = terminal.transform.position;
        transform.rotation = terminal.transform.rotation;
    }

    public void Disconnect()
    {
        if (connectedTerminal != null)
        {
            connectedTerminal.connectedWire = null;
            connectedTerminal = null;
        }
    }
}
