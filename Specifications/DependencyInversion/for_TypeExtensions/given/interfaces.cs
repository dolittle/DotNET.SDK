// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DependencyInversion.for_TypeExtensions.given;

public interface non_generic_interface {}
public interface generic_interface<T> {}
public interface non_generic_interface_implementing_generic_interface : generic_interface<int> {}
public interface generic_interface_implementing_generic_interface<T> : generic_interface<T> {}
public interface non_generic_interface_implementing_non_generic_interface : non_generic_interface {}
public interface generic_interface_implementing_non_generic_interface<T> : non_generic_interface {}