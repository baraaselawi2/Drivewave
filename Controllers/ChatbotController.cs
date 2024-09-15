using Microsoft.AspNetCore.Mvc;

public class ChatbotController : Controller
{
    // Action method for rendering the chatbot view
    public ActionResult Index()
    {
        return View();
    }

    // Action method for handling user input and returning a bot response
    [HttpPost]
    public JsonResult GetResponse(string userMessage)
    {
        string botResponse = GenerateBotResponse(userMessage);
        return Json(new { response = botResponse });
    }

    // Method to generate responses based on user input
    private string GenerateBotResponse(string userMessage)
    {
        userMessage = userMessage.ToLower(); // Normalize input for consistent matching

        if (userMessage.Contains("hello") || userMessage.Contains("hi"))
        {
            return "Hi there! How can I assist you today?";
        }
        else if (userMessage.Contains("book a car"))
        {
            return "To book a car, follow these steps: 1. Select your car type. 2. Enter pick-up and drop-off locations. 3. Choose pick-up and drop-off dates and times. 4. Click 'Book Now' to complete your reservation.";
        }
        else if (userMessage.Contains("cancellation policy"))
        {
            return "Our cancellation policy allows you to cancel your reservation up to 24 hours before the scheduled pick-up time for a full refund. Cancellations made within 24 hours may incur a fee.";
        }
        else if (userMessage.Contains("change reservation"))
        {
            return "To change your reservation, please contact our support team with your reservation details, and we will assist you with the changes.";
        }
        else if (userMessage.Contains("included in the rental"))
        {
            return "Our rental includes the car, basic insurance, and 24/7 roadside assistance. Additional options such as GPS or child seats can be added for an extra fee.";
        }
        else if (userMessage.Contains("contact support"))
        {
            return "You can contact our support team via email at support@rental.com or call us at 0787584398.";
        }
        else if (userMessage.Contains("change password"))
        {
            return "You can contact our support team via email at support@rental.com or call us at 0787584398 to change your password.";
        }
        else if (userMessage.Contains("operating hours"))
        {
            return "Our office is open from Monday to Friday, 9 AM to 6 PM. We are closed on weekends and public holidays.";
        }
        else if (userMessage.Contains("payment methods"))
        {
            return "We accept various payment methods including credit cards, debit cards, and PayPal.";
        }
        else if (userMessage.Contains("customer reviews"))
        {
            return "You can read customer reviews on our website under the 'Reviews' section or on our social media pages.";
        }
        else if (userMessage.Contains("fleet information"))
        {
            return "We offer a diverse fleet of vehicles including economy, luxury, and SUVs. For more details, please visit our 'Fleet' page.";
        }
        else if (userMessage.Contains("membership benefits"))
        {
            return "Our membership program offers various benefits such as discounted rates, priority booking, and exclusive offers. Learn more on our 'Membership' page.";
        }
        else if (userMessage.Contains("age requirement"))
        {
            return "You must be at least 21 years old to rent a car with us. Drivers under 25 may be subject to a young driver fee.";
        }
        else if (userMessage.Contains("insurance coverage"))
        {
            return "We offer several insurance options, including collision damage waiver (CDW), theft protection, and personal accident insurance. Details can be found on our 'Insurance' page.";
        }
        else if (userMessage.Contains("fuel policy"))
        {
            return "Our fuel policy requires you to return the car with the same amount of fuel as at pick-up. A refueling charge will apply if the tank is not full.";
        }
        else if (userMessage.Contains("roadside assistance"))
        {
            return "24/7 roadside assistance is included with your rental for any issues such as breakdowns or flat tires.";
        }
        else if (userMessage.Contains("extra drivers"))
        {
            return "You can add extra drivers to your rental for an additional fee. All additional drivers must meet the same age and license requirements as the primary driver.";
        }
        else if (userMessage.Contains("late fees"))
        {
            return "Late fees apply if you return the car later than the agreed-upon time. Fees are calculated on an hourly or daily basis depending on how late the return is.";
        }
        else if (userMessage.Contains("reservation confirmation"))
        {
            return "Once your reservation is confirmed, you will receive a confirmation email with all the details. Please check your inbox and spam folder for the confirmation email.";
        }
        else if (userMessage.Contains("discount codes"))
        {
            return "Discount codes can be applied during the booking process. Enter the code in the provided field to receive your discount.";
        }
        else if (userMessage.Contains("vehicle condition"))
        {
            return "All our vehicles undergo regular maintenance and are inspected before each rental to ensure they are in excellent condition.";
        }
        else if (userMessage.Contains("international travel"))
        {
            return "For international travel, please check the specific requirements and restrictions for driving in other countries. Contact our support team for more information.";
        }
        else if (userMessage.Contains("driver's license"))
        {
            return "You must present a valid driver's license at the time of pick-up. An international driver's license may be required if you're traveling from abroad.";
        }
        else if (userMessage.Contains("what to do in case of an accident"))
        {
            return "In case of an accident, please contact local authorities immediately and inform our support team. Follow the instructions provided in your rental agreement for further steps.";
        }
        else if (userMessage.Contains("can you help me with my booking"))
        {
            return "I can assist you with your booking. Please provide the details of your booking or let me know what specific help you need.";
        }
        else
        {
            return "Contact With Us To Know The Answer To Your Questions";
        }
    }
}
