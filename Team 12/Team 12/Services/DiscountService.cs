namespace Team_12.Services
{
    public class DiscountService
    {
        public decimal CalculateDiscount(List<string> members, decimal totalCost)
        {
            decimal familyDiscount = totalCost * 0.20m; // 20% discount for family
            decimal individualDiscount = 0m;

            foreach (var member in members)
            {
                switch (member)
                {
                    case "Pensioner":
                        individualDiscount += totalCost * 0.25m; // 25% for pensioners
                        break;
                    case "Student":
                        individualDiscount += totalCost * 0.15m; // 15% for students
                        break;
                    case "Teenager":
                        individualDiscount += totalCost * 0.10m; // 10% for teenagers (13-19 years)
                        break;
                    case "Child":
                        individualDiscount += totalCost * 0.25m; // 25% for children (0-12 years old)
                        break;
                    default:
                        break;
                }
            }

            // Apply family discount only if it's better than individual discounts
            return Math.Min(familyDiscount, individualDiscount);
        }
    }

}
