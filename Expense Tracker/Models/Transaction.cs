﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Range(1, 10000000000000, ErrorMessage = "Amount should be greater than 0.")]
        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {


                if (Category == null || Category.Type == "Expense")
                {
                    if (Amount < 0)
                    {
                        return "+ " + (Amount * -1).ToString();
                    }
                    else
                    {
                        return "- " + Amount.ToString();
                    }
                }
                else if (Category.Type == "Income")
                {
                    return "+ " + Amount.ToString();
                }
                else
                {
                    return "Unknown";
                }

            }
        }

    }
}
