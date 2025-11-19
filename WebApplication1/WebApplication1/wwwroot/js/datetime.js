function updateDateTime() {
    const now = new Date();
    const dateTimeStr = now.toLocaleDateString('uk-UA', {
        day: '2-digit',
        month: 'long',
        hour: '2-digit',
        minute: '2-digit'
    });

    const element = document.getElementById('currentDateTime');
    if (element) {
        element.textContent = dateTimeStr;
    }
}

function startSmartDateTimeUpdater() {
    const now = new Date();
    const seconds = now.getSeconds();
    const delayToNextMinute = (60 - seconds) * 1000;

    // Оновлюємо на початку кожної хвилини (коли секунди = 00)
    setTimeout(function () {
        updateDateTime();
        setInterval(updateDateTime, 60000); // Далі кожну хвилину
    }, delayToNextMinute);

    // Показуємо поточний час відразу
    updateDateTime();
}

document.addEventListener('DOMContentLoaded', startSmartDateTimeUpdater);