from aiogram import Bot, types
from aiogram.dispatcher import Dispatcher
from aiogram.utils import executor
import asyncio

from config import *
from notificator import *
from helperMethods import checkChatId, getSleepTime

loop = asyncio.get_event_loop()
bot = Bot(TOKEN, loop)
dp = Dispatcher(bot)

@dp.message_handler(commands=['start'])
async def process_start_command(message: types.Message):
        await message.reply("Hi!\nBot on duty!")

# ---------------------------------------- help -------------------------------------------------#
@dp.message_handler(commands=['help'])
async def process_help_command(message: types.Message):
    if checkChatId(message.chat.id) == True:
        await message.reply(HELP_MESSAGE_RU)
    else:
        await message.reply("Access denied.")

@dp.message_handler(commands=['samples'])
async def process_help_samples(message: types.Message):
    if checkChatId(message.chat.id) == True:
        await message.reply(HELP_SAMPLES_MESSAGE_RU)
    else:
        await message.reply("Access denied.")

# -------------------------------------- Basic CRUD with DB --------------------------------------#
@dp.message_handler(commands=['getAll'])
async def getAllEmployees(message: types.Message):
    if checkChatId(message.chat.id) == True:
        employeeInfo = await DBModel.getAllEmployees(forPrint=True)
        await message.reply(employeeInfo)
    else:
        pass

@dp.message_handler(commands=['findEmployee'])
async def searchEmployee(message: types.Message):
    if checkChatId(message.chat.id) == True:
        employeeInfo = await DBModel.findEmployeeById(message.get_args())
        await message.reply(employeeInfo)
    else:
        pass

@dp.message_handler(commands=['addEmployee'])
async def addEmployee(message: types.Message):
    if checkChatId(message.chat.id) == True:
        response = await DBModel.addEmployee(message.get_args())
        await message.reply(response)
    else:
        pass
@dp.message_handler(commands=['deleteEmployee'])
async def delEmployee(message: types.Message):
    if checkChatId(message.chat.id) == True:
        response = await DBModel.deleteEmployee(message.get_args())
        await message.reply(response)
    else:
        pass
# --------------------------------------------------------------------------------------------------#
@dp.message_handler(commands=['checkBirthday'])
async def checkBirthday(message: types.Message):
    if checkChatId(message.chat.id) == True:
        await sendListByBotCommand()
        await message.reply("В ближайшие 10 дней больше нет именинников.", disable_notification=True)
    else:
        pass

async def periodic():
    while True:
        await birthDayMakeAlert()
        sleepFor = getSleepTime(MORNING_ALERT_HOUR, DAY_ALERT_HOUR)
        await asyncio.sleep(sleepFor)

if __name__ == '__main__':
    dp.loop.create_task(periodic())
    executor.start_polling(dp)





