using System.Collections;
using System.Collections.Generic;

public class SidePot
{
    private List<AbstractContestant> _applicableContestants;
    private int _sidePotAmount;

    public SidePot(List<AbstractContestant> applicalbleContestants, int sidePotAMount)
    {
        _applicableContestants = applicalbleContestants;
        _sidePotAmount = sidePotAMount;
    }

    public int pot
    {
        get {return _sidePotAmount;}
    }

    public List<AbstractContestant> contenders
    {
        get {return _applicableContestants;}
    }
}
