﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllReady.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Display(Name = "Campaign")]
        public int CampaignId { get; set; }

        public Campaign Campaign { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Activity Type")]
        public ActivityTypes ActivityType { get; set; }    
        
        public int NumberOfVolunteersRequired { get; set; }

        [Display(Name = "Start date")]
        public DateTimeOffset StartDateTime { get; set; }

        [Display(Name = "End date")]
        public DateTimeOffset EndDateTime { get; set; }

        public Location Location { get; set; }

        public List<AllReadyTask> Tasks { get; set; } = new List<AllReadyTask>();

        public List<ActivitySignup> UsersSignedUp { get; set; } = new List<ActivitySignup>();

        public ApplicationUser Organizer { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Display(Name = "Required skills")]
        public List<ActivitySkill> RequiredSkills { get; set; } = new List<ActivitySkill>();
    }
}