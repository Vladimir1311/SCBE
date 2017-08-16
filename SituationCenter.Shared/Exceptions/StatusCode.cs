using System;
using System.ComponentModel;

namespace SituationCenter.Shared.Exceptions
{
    public enum StatusCode
    {
        [Description("Всё хорошо")]
        OK,
        [Description("Произошло несколько ошибок")]
        ComplexError,
        [Description("Неизвестная ошибка")]
        UnknownError,
        [Description("Слишком много запросов (Не ддось, плз)")]
        TooManyRequests,
        [Description("Профилактические работы на сервере, сорри")]
        ProfilacticWorks,
        [Description("Сервер отказал, ждем перемен")]
        ServerError,
        [Description("Данный email занят (регистрация)")]
        EmailBusy,
        [Description("Некорректный формат email")]
        IncorrectEmail,
        [Description("Неправильный формат пароля при регистрации")]
        IncorrectPassword,
        [Description("На данный телефон уже зарегистрирован аккаунт")]
        PhoneBusy,
        [Description("Неправильный формат телефона")]
        IncorrectPhone,
        [Description("Не правильные Имя/Фамилия при регистрации")]
        IncorrectFullName,
        [Description("Пароли не совпадают")]
        IncorrectConfirmPassword,
        [Description("Неправильный логин/пароль")]
        AuthorizeError,
        [Description("Название комнаты уже занято")]
        RoomNameBusy,
        [Description("Неерный формат имени комнаты")]
        IncorrectRoomName,
        [Description("Неправильный пароль для комнаты при создании/входе")]
        IncorrectRoomPassword,
        [Description("Такой комнаты не существует")]
        DontExistRoom,
        [Description("Комната заполнена")]
        RoomFilled,
        [Description("Функция не реализована")]
        NotImplementFunction,
        [Description("Неверное указание максимального количества членов комнаты")]
        MaxPeopleCountInRoomIncorrect,
        [Description("Вы находитесь в комнате")]
        PersonInRoomAtAWrongTime,
        [Description("Неправильные переданные параметры")]
        ArgumentsIncorrect,
        [Description("Нет доступа к чему либо. Например попытка присоединения к комнате без приглашения")]
        AccessDenied

    }
}
