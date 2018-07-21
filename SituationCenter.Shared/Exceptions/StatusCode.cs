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
        AccessDenied,

        [Description("Попытка создать комнату без приглашенных. Необходимо пригласить как минимум себя")]
        EmptyInvationRoom,

        [Description("Попытка сделать что-то с текущей комнатой, хотя клиент в комнате не находится")]
        YouAreNotInRoom,

        [Description("Попытка выполнения операции над комнатой не того типа")]
        IncrorrecrTargetRoomType,

        [Description("Слишком длинное название комнаты (максимум - 32 символа)")]
        TooLongRoomName,

        [Description("Невалидный токен доступ")]
        Unauthorized,
        [Description("Неверный Refresh токен")]
        IncorrectRefreshToken
    }
}