﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using sln_SingelApartment.ViewModels;
using sln_SingleApartment.Models;

namespace sln_SingelApartment.ViewModels
{
    public class CRoomStyleViewModel
    {
        public RoomStyle entity_roomstyle { get; set; }

        //需要的資料
        [DisplayName("房型序號")]
        public int ID { get { return this.entity_roomstyle.ID; } }
        [DisplayName("房型名稱")]
        public string roomStyleName { get { return this.entity_roomstyle.RoomStyleName; } }
       

       

    }
}