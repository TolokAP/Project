
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

using TMPro;

public class LogMessage : EntityEventListener<IPlayer>
{
    List<string> logMessage = new List<string>();
    [SerializeField] private Demo_Chat m_Chat;

    public TMP_Text WarningMessage;
    public Animator animator;


    public override void Attached()
    {
        m_Chat = GameObject.FindGameObjectWithTag("ChatGO").GetComponent<Demo_Chat>();
        WarningMessage = m_Chat.GetComponentInChildren<TMP_Text>();
        animator = WarningMessage.GetComponent<Animator>();
    }
    public override void OnEvent(LogEvent evnt)
    {
        if (evnt.FromSelf)
        {
            if (evnt.Warning)
            {
                WarningMessage.text = evnt.Message;
                animator.SetTrigger("Message");

            }
            else
            {
                logMessage.Insert(0, evnt.Message);
                AddMessages(evnt.Message);
            }
        }
    }

   


    [ContextMenu("Add Messages")]
    public void AddMessages(string value)
    {
        if (m_Chat != null)
        {
            m_Chat.ReceiveChatMessage(1, value);
          

        }
    }
}
