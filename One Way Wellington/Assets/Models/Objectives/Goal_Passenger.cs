using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Passenger : Goal
{

    public Goal_Passenger(string title, int numberOfPassengers)
    {
        this.title = title;
        this.goalAmount = numberOfPassengers;
    }

    public override bool CheckComplete()
    {
        if (JourneyController.Instance.passengersDeliveredLastJourney >= goalAmount)
        {
            return true;
        }
        return false;
    }
}
