import datetime
from config import CHAT_ID, MORNING_ALERT_HOUR, DAY_ALERT_HOUR

def getDaysBetween(dateOfBirth):
    today = datetime.date.today()
    dateTuple = tuple(map(int, dateOfBirth.split('-')))
    dateOfBirthNoYear = datetime.date(today.year, dateTuple[0], dateTuple[1])
    daysBetween = dateOfBirthNoYear - today
    return daysBetween.days

def printAllEmployees(rows):
    formatRow = []
    for r in rows:
        buferRow = str(str(r[0])+" "+str(r[2])+" "+str(r[1])+" "+str(r[4])+"\n")
        formatRow.append(buferRow)
    formatRow = ' '.join(formatRow)
    return formatRow

def checkChatId(chatId):
    if chatId == CHAT_ID:
        return True
    else:
        return False

def getSleepTime(morningHour, dayHour):
    now = datetime.datetime.now()
    hoursList =[]
    hoursList.append(morningHour - int(now.hour))
    hoursList.append(dayHour - int(now.hour))
    try:
        hour = min(h for h in hoursList if h >= 0)
    except:
        hour = min(h for h in hoursList if h < 0)
        hour = 24 + hour
    minutes = 60 - now.minute
    if hour > 0:
        seconds = (hour - 1) * 3600 + minutes * 60
        return seconds
    else:
        return minutes * 60


