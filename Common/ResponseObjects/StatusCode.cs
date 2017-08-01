using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.ResponseObjects
{
    public enum StatusCode
    {
        [StatusCodeDescription("Всё хорошо")]
        OK,
        [StatusCodeDescription("Произошло несколько ошибок")]
        ComplexError,
        [StatusCodeDescription("Неизвестная ошибка")]
        UnknownError,
        [StatusCodeDescription("Слишком много запросов (Не ддось, плз)")]
        TooManyRequests,
        [StatusCodeDescription("Профилактические работы на сервере, сорри")]
        ProfilacticWorks,
        [StatusCodeDescription("Сервер отказал, ждем перемен")]
        ServerError,
        [StatusCodeDescription("Данный email занят (регистрация)")]
        EmailBusy,
        [StatusCodeDescription("Некорректный формат email")]
        IncorrectEmail,
        [StatusCodeDescription("Неправильный формат пароля при регистрации")]
        IncorrectPassword,
        [StatusCodeDescription("На данный телефон уже зарегистрирован аккаунт")]
        PhoneBusy,
        [StatusCodeDescription("Неправильный формат телефона")]
        IncorrectPhone,
        [StatusCodeDescription("Не правильные Имя/Фамилия при регистрации")]
        IncorrectFullName,
        [StatusCodeDescription("Пароли не совпадают")]
        IncorrectConfirmPassword,
        [StatusCodeDescription("Неправильный логин/пароль")]
        AuthorizeError,
        [StatusCodeDescription("Название комнаты уже занято")]
        RoomNameBusy,
        [StatusCodeDescription("Неерный формат имени комнаты")]
        IncorrectRoomName,
        [StatusCodeDescription("Неправильный пароль для комнаты при создании/входе")]
        IncorrectRoomPassword,
        [StatusCodeDescription("Такой комнаты не существует")]
        DontExistRoom,
        [StatusCodeDescription("Комната заполнена")]
        RoomFilled,
        [StatusCodeDescription("Функция не реализована")]
        NotImplementFunction,
        [StatusCodeDescription("Неверное указание максимального количества членов комнаты")]
        MaxPeopleCountInRoomIncorrect,
        [StatusCodeDescription("Вы находитесь в комнате")]
        PersonInRoomAtAWrongTime,
        [StatusCodeDescription("Неправильные переданные параметры")]
        ArgumentsIncorrect

    }



    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class StatusCodeDescription : Attribute
    {
        readonly string description;

        // This is a positional argument
        public StatusCodeDescription(string description) => 
            this.description = description;

        public string Description
        {
            get => description;
        }
    } 
}
