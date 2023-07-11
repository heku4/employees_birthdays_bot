namespace BirthdayBot.Core.Models;

public class Employee
{
    private readonly int _id;
    private readonly string _name;
    private readonly int _birthDay;
    private readonly int _birthMonth;
    private readonly DateOnly _currentYearBirthDay;

    public int DaysBeforeBirthDate { get; set; }

    public Employee(int id, string name, int birthDay, int birthMonth)
    {
        _id = id;
        _name = name;
        _birthDay = birthDay;
        if (_birthDay < 1 || _birthDay > 31)
        {
            throw new ArgumentException($"Check birth day value of {_name}, id: {_id}");
        }

        _birthMonth = birthMonth;
        if (_birthDay < 1 || _birthDay > 11)
        {
            throw new ArgumentException($"Check birth month value of {_name}, id: {_id}");
        }

        _currentYearBirthDay = GetCurrentBirthday();


        DaysBeforeBirthDate = GetDaysBeforeBirthday();
    }

    public override string ToString()
    {
        return $"{_id}:  {_name}   {_currentYearBirthDay.ToString("M")}";
    }

    private DateOnly GetCurrentBirthday()
    {
        var currentYear = DateTime.Now.Year;
        var birthdayDate = new DateOnly(currentYear, _birthMonth, _birthDay);

        // special check for 29 of February
        if (_birthDay == 29 && _birthMonth == 2)
        {
            if (DateTime.IsLeapYear(currentYear))
            {
                return birthdayDate;
            }

            return new DateOnly(currentYear, 3, 1);
        }


        return birthdayDate;
    }

    private int GetDaysBeforeBirthday()
    {
        var currentDateTime = DateTime.Now;
        var currentDate = new DateOnly(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day);

        var daysDifference = currentDate.DayNumber - _currentYearBirthDay.DayNumber;

        return daysDifference;
    }
}