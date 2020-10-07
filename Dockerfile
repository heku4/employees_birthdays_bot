FROM python:3.8

WORKDIR /app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

VOLUME /app
COPY bot.py /app
COPY headpoint_bdays.db /app
COPY dbModel.py /app
COPY helperMethods.py /app
COPY notificator.py /app
COPY debug_config.py /app

CMD [ "python", "/app/bot.py" ]
