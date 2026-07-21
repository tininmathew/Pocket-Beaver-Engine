#!/bin/bash

array=('A' 'B' 'C' 'D' 'E' 'F' 'G' 'H' 'I' 'K' 'L' 'M' 'N' 'O' 'P' 'Q' 'R' 'S' 'T' 'U' 'V' 'W' 'X' 'Y' 'Z' '1' '2' '3' '4' '5' '6' '7' '8' '9' '0' 'a' 'b' 'c' 'd' 'e' 'f' 'g' 'h' 'i' 'k' 'l' 'm' 'n' 'o' 'p' 'q' 'r' 's' 't' 'u' 'v' 'w' 'x' 'y' 'z' '!' '@' '#' '$' '%' '^' '&' '*' '(' ')' '_' '+' '-' '=' '[' ']' '\' ';' '`' '\,' '\.' '/' '\<' '\>' '\?' ':' '\"' '{' '}' '\|')

counter=0

for sym in "${array[@]}"; do
    # Форматируем счетчик в 4 знака (0 -> 0000, 1 -> 0001)
    old_name=$(printf "%04d.png" $counter)
    new_name="${sym}.png"

    # Проверяем, существует ли исходный файл
    if [ -f "$old_name" ]; then
        echo "Переименование: $old_name -> $new_name"
        mv "$old_name" "$new_name"
    else
        echo "Предупреждение: файл $old_name не найден."
    fi

    # Увеличиваем счетчик на 1
    ((counter++))
done
echo $counter