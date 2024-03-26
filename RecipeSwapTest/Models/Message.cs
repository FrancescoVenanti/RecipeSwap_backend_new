using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RecipeSwapTest.Models;

public partial class Message
{
    [Key]
    [Column("MessageID")]
    public int MessageId { get; set; }

    [Column("SenderUserID")]
    public int SenderUserId { get; set; }

    [Column("ReceiverUserID")]
    public int ReceiverUserId { get; set; }

    public string MessageContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Timestamp { get; set; }

    public bool? IsRead { get; set; }

    [ForeignKey("ReceiverUserId")]
    [InverseProperty("MessageReceiverUsers")]
    public virtual User ReceiverUser { get; set; } = null!;

    [ForeignKey("SenderUserId")]
    [InverseProperty("MessageSenderUsers")]
    public virtual User SenderUser { get; set; } = null!;
}
