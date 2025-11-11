# Projections

This is a quick set of data types 
I think might be generally useful.
They are designed to provide public facing versions of
collections that have modified versions of the internals.

For example if you have a `Dictionay<object, (v1, v2, v3)>`
and you want to expose a `IReadonlyDictionary<object, (v1, v2)`
but specifically not `v3`. You could build a system that updates
an intermediary dictionary every time the primary one updates, 
or you could just use a projection dictionary. provide a function
to convert `(v1, v2, v3)`  to `(v1, v2)` and side step the issue. 
Minimal data duplication (Sets required some duplication, 
but the other have none.)

It might be easer to think of these as a view from a SQL database,
not having any of their own data, but a way of looking at the database.

Please let me know if there are any other data type I should do or 
if the are improvements that can be made to the ones ive made.