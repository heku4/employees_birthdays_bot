TOKEN =''
CHAT_ID = 0
MORNING_ALERT_HOUR = 11
DAY_ALERT_HOUR = 15
HELP_MESSAGE =  "УВАЖАЕМЫЙ ПОЛЬЗОВАТЕЛЬ! ВНИМАТЕЛЬНО ИСПОЛЬЗУЙ КОМАНДЫ, НЕ ВСЕ ПРОВЕРКИ НАСТРОЕНЫ\n\n"\
                "For help type /help;\n"\
                "To get all employees type /getAll;\n"\
                "To find employee by id use $/findEmployee NAME SECONDNAME$;\n"\
                "To add new employee use $/addEmployee NAME SECONDNAME FATHERsNAME BDay(MM-DD)$;\n"\
                "To delete employee from DB type $/deleteEmployee ID$;\n"\
                "To check nearest Birthdays try /checkBirthday."

HELP_MESSAGE_RU = "УВАЖАЕМЫЙ ПОЛЬЗОВАТЕЛЬ! ВНИМАТЕЛЬНО ИСПОЛЬЗУЙ КОМАНДЫ\n"\
                  "Увидеть иструкцию - /help;\n"\
                  "Получить список всех сотрудников - /getAll\n"\
                  "Получить информацию о сотруднике - $/findEmployee ИМЯ ФАМИЛИЯ$\n"\
                  "Добавить сотрудника - $/addEmployee ИМЯ ФАМИЛИЯ ОТЧЕСТВО ДЕНЬ_РОЖДЕНИЯ(MM-ДД)$\n"\
                  "Уволить сотрудника - $/deleteEmployee ID$\n"\
                  "Проверить ближайшие Дни рождения (до 10 дней) - /checkBirthday."

ERR_DB_MESSAGE = 'Error: problem with database'
ERR_DB_MESSAGE_RU = 'Ошибка: проблема с базой данных'
ERR_DB_SEARCH_MESSAGE = 'Found nothing or another exception'
ERR_DB_SEARCH_MESSAGE_RU = "Ничего не найдено или другая ошибка"