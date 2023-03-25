using UnityEngine;
using UnityEngine.UI;

public class StatsWindowUI : MonoBehaviour
{
    [SerializeField] private Text _level;
    [SerializeField] private Text _curExperience;
    [SerializeField] private Text _pointExperience;
    [SerializeField] private Text _curHP;
    [SerializeField] private Text _curMana;
    
    internal void SetStates(UnitStats unitStats)
    {
        _level.text = "Уровень: " + unitStats.level.ToString();
        
        _curExperience.text = "Опыт: " + unitStats.curExperience.ToString() + " / " + unitStats.maxExperience.ToString();
        
        _pointExperience.text = "Очки опыта: " + unitStats.pointExperience.ToString();
        
        _curHP.text = "Здоровье: " + unitStats.curHP.ToString() + " / " + unitStats.maxHP.ToString();
        
        _curMana.text = "Мана: " + unitStats.curMana.ToString() + " / " + unitStats.maxMana.ToString();
    }
}
