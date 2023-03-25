using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpWindowUI : MonoBehaviour
{
    private AbstractBehavior targetHumanForHelp;

    public void CommandReviveTarget()
    {
        if(!CheckTarget())return;

        targetHumanForHelp.Revive();
    }

    public void CommandAttackTarget()
    {
        if(!CheckTarget())return;

        Unit tar = (Unit)targetHumanForHelp;
        tar.CommandAttackTheTarget();
    }

    public void CommandHealingTarget()
    {
        if(!CheckTarget())return;

        targetHumanForHelp.Healing(1000000);
    }

    public void CommandKillTarget()
    {
        // if(!CheckTarget())return;

        // targetHumanForHelp.TakeDamage(pLayerController, targetHumanForHelp.GetCurHP());
    }

     public void RestartScene(string value)
    {
        SceneManager.LoadScene(value);
    }

    public void CommandTakeTarget()
    {
        // targetItemForHelp.GetComponent<ItemOnstreet>().TakeItem(pLayerController.Chest);
        // targetItemForHelp = null;
        // Debug.Log("takeTarget");
    }

    private bool CheckTarget()
    {
        if(!targetHumanForHelp)
        {
            Debug.Log("GameManager! Нет таргета");
            return false;
        }
        return true;
    }
}
