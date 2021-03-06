using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class PlayerContestant : AbstractContestant
{
    //Include a event to deactivate some UI features once the player makes a decision

    public event MoneyEventHandler MoneyChanged;
    public event PlayerMadeDecisionHandler PlayerDecided;

    private void Awake() {
        _hand = GetComponent<Hand>();
        _isPlayer = true;
    }

    public override void ChangeMoney(int money)
    {
        Debug.Log("Money: " + money + "+" + _money);
        _money += money;
        if (_money < 0)
            _money = 0;

        //Debug.Log("Money after: " + _money);

        MoneyChanged(this, new MoneyEventArgs(_money));
    }

    public IPokerCommand OnDecisionMade(object sender, PlayerDecisionEventArgs p)
    {
        return p.playerDecision;
    }

    public override IPokerCommand MakeDecision()
    {
        throw new System.NotImplementedException();
    }
}
