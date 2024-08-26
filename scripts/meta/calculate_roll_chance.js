const iterations = 100000
const minimum_bonus = 0
const maximum_bonus = 10
const resolve_ties = false
const advantage = 1
const split_for_google = true

let results = []

for (let bonus_1 = minimum_bonus; bonus_1 <= maximum_bonus; bonus_1++) {
    results[bonus_1] = {}

    for (let bonus_2 = minimum_bonus; bonus_2 <= maximum_bonus; bonus_2++) {
        let result = results[bonus_1][bonus_2] = { wins: 0, ties: 0, losses: 0, }

        for (let i = 0; i < iterations; i++) {
            let roll_1 = 0

            for (let i = 0; i <= advantage; i++) {
                var roll =  Math.ceil(Math.random() * 10)
                roll_1 = roll > roll_1 ? roll : roll_1
            }

            const roll_2 = Math.ceil(Math.random() * 10)
    
            const total_1 = roll_1 + bonus_1
            const total_2 = roll_2 + bonus_2
    
            if (total_1 === total_2) {
                if (advantage > 0) result.wins++
                else if (!resolve_ties || bonus_1 === bonus_2) result.ties++
                else bonus_1 > bonus_2 ? result.wins++ : result.losses++
            }
            else total_1 > total_2 ? result.wins++ : result.losses++
        }
    }
}

let csv = split_for_google ? "=SPLIT(\"," : ','

for (let i = minimum_bonus; i <= maximum_bonus; i++) {
    csv += `${i},`
}

if (split_for_google) csv += "\",\",\")"
csv += '\n'

for (let i = minimum_bonus; i <= maximum_bonus; i++) {
    if (split_for_google) csv += "=SPLIT(\","
    csv += `${i},`

    for (let j = minimum_bonus; j < maximum_bonus; j++) {
        const result = results[i][j]
        const percentage = Math.round((result.wins / iterations) * 100)
        csv += `${percentage},`
    }

    if (split_for_google) csv += "\",\",\")"
    csv += '\n'
}

console.log(csv)
