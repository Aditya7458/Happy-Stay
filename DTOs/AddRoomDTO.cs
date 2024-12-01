﻿using System.ComponentModel.DataAnnotations;

public class AddRoomDTO
{
    [Required]
    public int HotelID { get; set; }

    [Required]
    [StringLength(50)]
    public string? RoomSize { get; set; }

    [Required]
    public string? BedType { get; set; }

    [Required]
    public int MaxOccupancy { get; set; }

    [Required]
    public decimal BaseFare { get; set; }

    public bool IsAC { get; set; } = false;

    public bool AvailabilityStatus { get; set; } = true;

    [Required]
    public int CreatedBy { get; set; }
}