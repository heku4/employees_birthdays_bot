TOKEN = ""
CHAT_ID = 0
MORNING_ALERT_HOUR = 11
DAY_ALERT_HOUR = 15
DB_NAME = "bdays.db"
HELP_MESSAGE =  "For help type /help;\n"\
                "To get all employees type /getAll;\n"\
                "To find employee by id use $/findEmployee NAME SECONDNAME$;\n"\
                "To add new employee use $/addEmployee NAME SECONDNAME FATHERsNAME BDay(MM-DD)$;\n"\
                "To delete employee from DB type $/deleteEmployee ID$;\n"\
                "To check nearest Birthdays try /checkBirthday."

HELP_MESSAGE_RU = "Увидеть иструкцию - /help;\n"\
                  "Получить список всех сотрудников - /getAll\n"\
                  "Получить информацию о сотруднике - $/findEmployee ИМЯ ФАМИЛИЯ$\n"\
                  "Добавить сотрудника - $/addEmployee ИМЯ ФАМИЛИЯ ОТЧЕСТВО ДЕНЬ_РОЖДЕНИЯ(MM-ДД)$\n"\
                  "Уволить сотрудника - $/deleteEmployee ID$\n"\
                  "Проверить ближайшие Дни рождения (до 10 дней) - /checkBirthday\n"\
                  "Посмотреть примеры команд - /samples\n"

HELP_SAMPLES_MESSAGE_RU = "Примеры работы с информацией о сотрудниках\n" \
                          "/addEmployee Петр Петров Петрович 12-30\n"\
                          "/findEmployee Петр Петров\n" \
                          "/deleteEmployee 98\n"

ERR_DB_MESSAGE = "Error: problem with database"
ERR_DB_MESSAGE_RU = "Ошибка или проблема с базой данных"
ERR_DB_SEARCH_MESSAGE = "Found nothing or another exception"
ERR_DB_SEARCH_MESSAGE_RU = "Ничего не найдено или другая ошибка"
DONE_DB_MESSAGE = "Alldone! Enjoy!"
DONE_DB_MESSAGE_RU = "Готово! Наслаждайтесь!"