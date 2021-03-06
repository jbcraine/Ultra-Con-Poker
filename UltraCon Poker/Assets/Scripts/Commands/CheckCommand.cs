using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCommand : IPokerCommand
{
   private AbstractContestant _contestant;

   public CheckCommand(AbstractContestant contestant)
   {
       _contestant = contestant;
   }

   public void Execute()
   {
       _contestant.Check();
   }
}
