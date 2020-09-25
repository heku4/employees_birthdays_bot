import sqlite3
import os.path

from helperMethods import printAllEmployees
from config import *
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
db_path = os.path.join(BASE_DIR, "headpoind_bdays.db")
db = sqlite3.connect(db_path)
c = db.cursor()

class DBModel:
    async def getAllEmployees(forPrint):
        try:
            c.execute('SELECT * FROM Employees ORDER by Employees.BirthDay')
        except sqlite3.Error as e:
            print(e)
        rows = c.fetchall()
        if rows is None:
            return ERR_DB_SEARCH_MESSAGE_RU
        while True:
            if forPrint is True:
                formatRows = printAllEmployees(rows)
                return formatRows
            else:
                return rows

    async def addEmployee(newEmployeeData):
        data = tuple(map(str, newEmployeeData.split(' ')))
        try:
            c.execute('INSERT INTO Employees(Name, SecondName, FatherName, BirthDay) VALUES (?, ?, ?, ?)',
                      data)
            db.commit()
            return "Готово! Наслаждайтесь!"
        except sqlite3.Error as e:
            print(e)
            return ERR_DB_MESSAGE_RU

    async def findEmployeeById(employeeData):
        try:
            data = tuple(map(str, employeeData.split(' ')))
            c.execute('SELECT * FROM Employees WHERE Employees.Name = ? AND Employees.SecondName = ?', data)
        except sqlite3.Error as e:
            print(e)

        row = c.fetchone()

        if row is None:
            return (ERR_DB_SEARCH_MESSAGE_RU)
        while True:
            # row - tuple from table: name1 surname2 fatherName3 date_of_birth4
            employeeInfo = str("id="+str(row[0])+": "+str(row[2])+" "+str(row[1])+" "+str(row[3])+" "+str(row[4]))
            return employeeInfo

    async def deleteEmployee(employeeId):
        try:
            c.execute(f'DELETE FROM Employees WHERE Employees.Id = {employeeId}')
            db.commit()
            return f"The employee #{employeeId} has been deleted"
        except sqlite3.Error as e:
            print(e)
            return ERR_DB_MESSAGE_RU