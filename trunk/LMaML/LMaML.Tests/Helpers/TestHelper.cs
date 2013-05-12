using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.Helpers
{
    public static class TestHelper
    {
        /// <summary>
        /// Nulls the parameter test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void NullParameterTest<T>()
        {
            var type = typeof (T);
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    AssertThrows(constructor, parameter,
                        parameters.Select(x => x == parameter ? null : MakeMock(x.ParameterType)).ToArray());
                }
            }
        }

        /// <summary>
        /// Makes the mock.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">I don't know what I'm doing...</exception>
        public static object MakeMock(Type type)
        {
            var method = typeof(Mock).GetMethods().FirstOrDefault(x => x.IsGenericMethod && x.Name == "Create" && x.GetParameters().Length == 0);
            if (null == method)
                throw new NotSupportedException("I don't know what I'm doing...");
            method = method.MakeGenericMethod(type);
            return method.Invoke(null, new object[0]);
        }

        /// <summary>
        /// Makes the public transport mock.
        /// </summary>
        /// <param name="appEventBus">The app event bus.</param>
        /// <returns></returns>
        public static IPublicTransport MakePublicTransportMock(out IEventBus<IApplicationEvent> appEventBus)
        {
            var commandBus = Mock.Create<ICommandBus>();
            appEventBus = Mock.Create<IEventBus<IApplicationEvent>>();
            var mock = Mock.Create<IPublicTransport>();
            Mock.Arrange(() => mock.CommandBus).Returns(commandBus);
            Mock.Arrange(() => mock.ApplicationEventBus).Returns(appEventBus);
            return mock;
        }

        /// <summary>
        /// Makes the public transport mock.
        /// </summary>
        /// <param name="commandBus">The command bus.</param>
        /// <returns></returns>
        public static IPublicTransport MakePublicTransportMock(out ICommandBus commandBus)
        {
            commandBus = Mock.Create<ICommandBus>();
            var evMock = Mock.Create<IEventBus<IApplicationEvent>>();
            var mock = Mock.Create<IPublicTransport>();
            Mock.Arrange(() => mock.CommandBus).Returns(commandBus);
            Mock.Arrange(() => mock.ApplicationEventBus).Returns(evMock);
            return mock;
        }

        /// <summary>
        /// Makes the public transport mock.
        /// </summary>
        /// <param name="commandBus">The command bus.</param>
        /// <param name="appEventBus">The app event bus.</param>
        /// <returns></returns>
        public static IPublicTransport MakePublicTransportMock(out ICommandBus commandBus, out IEventBus<IApplicationEvent> appEventBus)
        {
            commandBus = Mock.Create<ICommandBus>();
            appEventBus = Mock.Create<IEventBus<IApplicationEvent>>();
            var mock = Mock.Create<IPublicTransport>();
            Mock.Arrange(() => mock.CommandBus).Returns(commandBus);
            Mock.Arrange(() => mock.ApplicationEventBus).Returns(appEventBus);
            return mock;
        }

        public static IReferenceAdapters CreateReferenceAdaptersMock(
            out IDataAdapter<Album> albumAdapter,
            out IDataAdapter<Artist> artistAdapter,
            out IDataAdapter<Title> titleAdapter,
            out IDataAdapter<Genre> genreAdapter,
            out IDataAdapter<Year> yearAdapter)
        {
            // Arrange
            var adaptersMock = Mock.Create<IReferenceAdapters>();
            albumAdapter = Mock.Create<IDataAdapter<Album>>();
            artistAdapter = Mock.Create<IDataAdapter<Artist>>();
            titleAdapter = Mock.Create<IDataAdapter<Title>>();
            genreAdapter = Mock.Create<IDataAdapter<Genre>>();
            yearAdapter = Mock.Create<IDataAdapter<Year>>();
            Mock.Arrange(() => adaptersMock.AlbumAdapter).Returns(albumAdapter);
            Mock.Arrange(() => adaptersMock.ArtistAdapter).Returns(artistAdapter);
            Mock.Arrange(() => adaptersMock.TitleAdapter).Returns(titleAdapter);
            Mock.Arrange(() => adaptersMock.GenreAdapter).Returns(genreAdapter);
            Mock.Arrange(() => adaptersMock.YearAdapter).Returns(yearAdapter);
            return adaptersMock;
        }

        /// <summary>
        /// Asserts the throws.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameters">The parameters.</param>
        private static void AssertThrows(ConstructorInfo constructor, ParameterInfo parameter, object[] parameters)
        {
            var ex = Assert.Throws<TargetInvocationException>(() => constructor.Invoke(parameters));
            Assert.IsInstanceOf<ArgumentNullException>(ex.InnerException);
            var inner = (ArgumentNullException) ex.InnerException;
            Assert.AreEqual(inner.ParamName, parameter.Name);
        }

        /// <summary>
        /// Asserts the property changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="properties">The properties.</param>
        public static void AssertPropertyChanged(INotifyPropertyChanged target, Action action,
                                                 params string[] properties)
        {
            // Arrange.....
            var changed = new List<string>();
            target.PropertyChanged += (sender, args) => changed.Add(args.PropertyName);

            // Act
            action();

            // Assert
            Assert.That(properties.All(changed.Contains));
        }
    }
}
