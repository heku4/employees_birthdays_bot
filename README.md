Telegram bot<br />
## Birthday notificator<br />
This bot send notification about Employee's Birthday.
Also it can execute some commands.

Steps to run bot:

Clone project. To pull all packages use:
 >*$ pip download -r requirements.txt*
2. Create database.
    You need to create DB with one table.
3. Put your database file in one directory with `bot.py`.
4. Create `config.py` file or use `debug_config.py`. Don't forget import it from `bot.py`.
4. Fill  `config.py` file: Bot token, chat ids, notification hours (24h format), DB name, DB table name.
4. Try it!

Or you can use *Dockerfile* to create & start your own container.
See [docker blogs](https://www.docker.com/blog/containerized-python-development-part-1/) for that.
