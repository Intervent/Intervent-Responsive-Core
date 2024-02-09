function ScoreColor(percent) {
    if (percent > 0) {
        //assign colors
        if (percent <= 79) {
            return 'red';
        } else if (percent <= 89) {
            return 'orange';
        } else {
            return 'green';
        }
    }
}

function getHAColor(calcAge, realAge) {
    if ((calcAge - realAge) > 3) {
        return 'red';
    }
    else if ((calcAge - realAge) > 0) {
        return 'orange';
    }
    else if (calcAge !== 0 && (calcAge - realAge <= 0)) {
        return 'green';
    }
    else {
        return 'blue';
    }
}