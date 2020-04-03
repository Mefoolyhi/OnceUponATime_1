﻿using System;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;

namespace OnceUponATime_1
{
    public class Player
    {
        [JsonProperty("Diamonds")]
        public int Diamonds { get; private set; }
        
        [JsonProperty("Keys")]
        public int Keys { get; private set; }
        
        [JsonProperty("TotalDays")]
        public int TotalDays { get; private set; }
        
        [JsonProperty("LastVisit")]
        public DateTime LastVisit { get; private set; }

        public Player(int diamonds, int keys, int totalDays = 0, DateTime lastVisit = default)
        {
            Diamonds = diamonds;
            Keys = keys;
            TotalDays = totalDays;
            LastVisit = lastVisit == default ? DateTime.Today : lastVisit;
        }

        public bool GetKey()
        {
            if (Keys <= 0) return false;
            Keys--;
            return true;

        }

        public bool SetKeys(int keys)
        {
            if (Keys + keys > 10)
            {
                Keys = 10;
                return false;
            }
            Keys += keys;
            return true;
        }

        public void SetDiamonds(int diamonds) => Diamonds += diamonds;
        
        public bool GetDiamonds(int diamonds)
        {
            if (Diamonds - diamonds < 0) return false;
            Diamonds -= diamonds;
            return true;
        }

        public bool CheckIfFirstVisit()
        {
            if (DateTime.Today.Date == LastVisit.Date && TotalDays > 0)
                return false;
            if (DateTime.Today.Date.Subtract(LastVisit.Date).Days == 1)
                TotalDays++;
            else
                TotalDays = 1;
            LastVisit = DateTime.Today;
            return true;
        }
    }
}