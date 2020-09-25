from aiogram import Bot

from config import TOKEN, CHAT_ID
bot = Bot(TOKEN)

from dbModel import DBModel
from helperMethods import getDaysBetween

async def birthDayMakeAlert():
        listOfNearestBirthday = []
        listToNotificate = await checkDaysDifference(listOfNearestBirthday)
        await prepareNotification(listToNotificate)

async def checkDaysDifference(listOfNearestBirthday):
    allEmployees = await DBModel.getAllEmployees(forPrint=False)
    for emp in allEmployees:
        delta = getDaysBetween(emp[4])  # 4th element is days until birthday
        if delta >= 0 and delta < 11:
            emp = list(emp)  # convert to list to use append method
            emp.append(delta)  # add in empInfo how many days until employee's birthday
            listOfNearestBirthday.append(emp)
    return listOfNearestBirthday

async def prepareNotification(listOfBirthday):
    if not listOfBirthday:
        pass
    else:
        for emp in listOfBirthday:
            if emp[5] == 0:
                responseMessage = f"Сегодня у {emp[2]} {emp[1]} День рождения!"
                await bot.send_message(text=responseMessage, chat_id=CHAT_ID)
            elif emp[5] == 5 or emp[5] == 10:
                responseMessage = f"Через {emp[5]} дней у {emp[1]} {emp[2]} День рождения!"
                await bot.send_message(text=responseMessage, chat_id=CHAT_ID)

async def sendListByBotCommand():
    listOfBirthdays=[]
    tenDaysList = await checkDaysDifference(listOfBirthdays)
    await prepareListToUser(tenDaysList)


async def prepareListToUser(listOfBirthdays):
    await bot.send_message(text="Проверяем дни рождения коллег...", chat_id=CHAT_ID)
    if not listOfBirthdays:
        pass
    else:
        for emp in listOfBirthdays:
            responseMessage = f"Через {emp[5]} дней у {emp[1]} {emp[2]}"
            await bot.send_message(text=responseMessage, chat_id=CHAT_ID)