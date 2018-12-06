using System;

namespace Application.Activities
{
  public class AttendeeDto
  {
    public string Username { get; set; }
    public DateTime DateJoined { get; set; }
    public string Image { get; set; }
    public bool IsHost { get; set; }
  }
}