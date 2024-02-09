function CalculateRisk() {
    var optimal = 0, notoptimal = 0, elavated = 0, major = 0, Gender, Age, Smoker, TChol, Sbp, SbpMed, Diabetes, heartDis, Race;
    var cholrisklimit = 180, cholrisklimit1 = 160, cholrisklimit2 = 200, cholrisklimit3 = 240, cholrisklimit4 = 280, cholrisklimit5 = 170;
    var hdlrisklimit1 = 35, hdlrisklimit2 = 45, hdlrisklimit3 = 50, hdlrisklimit4 = 60;
    var lifeTimeRisk, lifeTimeGoal, pCurrentFram, pAverageFram, pLowFram;
    var lnage, lntot, lnhdl, trlnsbp, ntlnsbp, age2, agetc, agehdl, agetsbp, agentsbp, agesmoke, agedm, s0_10, mnxb, predict, cvdpredict, ascvd;
    var goallntot, goallnhdl, goaltrlnsbp, goalntlnsbp, goalagetc, goalagehdl, goalagetsbp, goalagentsbp, goalagesmoke, goalagedm, goalpredict, goalcvdpredict;
    $('.item-label > .value').text('');
    $('.gauge-score').text('');
    $('.chart-message').text('');
    $('#chart').html('');
    $("#chart-info").hide();
    if ($('#toggle-gender:checked').length > 0) {
        Gender = 1;
    }
    else {
        Gender = 2;
    }
    if ($('#toggle-smoker:checked').length > 0) {
        Smoker = 1;
    }
    else {
        Smoker = 0;
    }
    if ($('#toggle-diabetes:checked').length > 0) {
        Diabetes = 1;
    }
    else {
        Diabetes = 0;
    }
    if ($('#toggle-khd:checked').length > 0) {
        heartDis = 1;
    }
    else {
        heartDis = 0;
    }
    if ($('#toggle-bpmed:checked').length > 0) {
        SbpMed = 1;
    }
    else {
        SbpMed = 0;
    }
    Age = $('#input-age').val();
    TChol = $('#input-cholesterol').val() / ConversionValue;
    HDL = $('#input-hdl').val() / ConversionValue;
    Sbp = $('#input-sbp').val();
    Dbp = $('#input-dbp').val();
    Race = $("#ddl-race").val();

    //show-hide warning icon
    if (selectedId == "option-heart-age" || selectedId == "option-risk-heart-disease") {
        if (HDL < hdlrisklimit4) {
            $('.hdl-warning').removeClass('no-warning');
        } else {
            $('.hdl-warning').addClass('no-warning');
        }
        if (TChol >= cholrisklimit2) {
            $('.cholesterol-warning').removeClass('no-warning');
        } else {
            $('.cholesterol-warning').addClass('no-warning');
        }
        if (Gender == 1) {
            if (Sbp > 129) {
                $('.sbp-warning').removeClass('no-warning');
            } else {
                $('.sbp-warning').addClass('no-warning');
            }
            if (Dbp > 84) {
                $('.dbp-warning').removeClass('no-warning');
            } else {
                $('.dbp-warning').addClass('no-warning');
            }
        }
        else {
            if (Dbp > 79) {
                $('.dbp-warning').removeClass('no-warning');
            } else {
                $('.dbp-warning').addClass('no-warning');
            }
        }
    }
    else if (selectedId == "option-lifetime-risk") {
        if (TChol >= cholrisklimit) {
            $('.cholesterol-warning').removeClass('no-warning');
        } else {
            $('.cholesterol-warning').addClass('no-warning');
        }
        if (Sbp >= 120) {
            $('.sbp-warning').removeClass('no-warning');
        } else {
            $('.sbp-warning').addClass('no-warning');
        }
    }
    else {
        if (HDL < hdlrisklimit3) {
            $('.hdl-warning').removeClass('no-warning');
        } else {
            $('.hdl-warning').addClass('no-warning');
        }
        if (TChol > cholrisklimit5) {
            $('.cholesterol-warning').removeClass('no-warning');
        } else {
            $('.cholesterol-warning').addClass('no-warning');
        }
        if (Sbp > 110) {
            $('.sbp-warning').removeClass('no-warning');
        } else {
            $('.sbp-warning').addClass('no-warning');
        }
    }
    if (selectedId == "option-heart-age" || selectedId == "option-risk-heart-disease") {
        if (Age > 74 || heartDis == 1) {
            if (selectedId == "option-heart-age") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(age74ha);
            }
            else if (selectedId == "option-risk-heart-disease") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(age74hd);
            }
            $('.item-label > .value').text(mobileMessage);
            return;
        }
        if (Age < 18) {
            if (selectedId == "option-heart-age") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(age18ha);
            }
            else if (selectedId == "option-risk-heart-disease") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(age18hd);
            }
            $('.item-label > .value').text(mobileMessage);
            return;
        }
        if (Gender == undefined || heartDis == undefined || Age == 0 || Smoker == undefined || Diabetes == undefined || TChol == 0 || HDL == 0 || Sbp == 0 || Dbp == 0) {
            if (selectedId == "option-heart-age") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(missingha);
            }
            else if (selectedId == "option-risk-heart-disease") {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(missinghd);
            }
            $('.item-label > .value').text(mobileMessage);
            return;
        }
    }
    else {
        if (selectedId == "option-lifetime-risk") {
            if (Age < 20 || Age > 59 || heartDis == 1) {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(ageal);
                $('.item-label > .value').text(mobileMessage);
                return;
            }
            if (Gender == undefined || heartDis == undefined || Age == 0 || Smoker == undefined || Diabetes == undefined || SbpMed == undefined || TChol == 0 || Sbp == 0) {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(missingalhs);
                $('.item-label > .value').text(mobileMessage);
                return;
            }
        }
        else {
            if (Age < 40 || Age > 79 || heartDis == 1) {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(agehs);
                $('.item-label > .value').text(mobileMessage);
                return;
            }
            if (Gender == undefined || heartDis == undefined || Age == 0 || Race == "" || Smoker == undefined || Diabetes == undefined || SbpMed == undefined || TChol == 0 || HDL == 0 || Sbp == 0) {
                clearChart();
                $(".chart-message").removeClass('hide');
                $(".chart-message").html(missingalhs);
                $('.item-label > .value').text(mobileMessage);
                return;
            }
        }
    }

    if (selectedId == "option-lifetime-risk") {
        if (TChol < cholrisklimit)
            optimal = 1
        else if (TChol >= cholrisklimit && TChol < cholrisklimit2)
            notoptimal = 1
        else if (TChol >= cholrisklimit2 && TChol < cholrisklimit3)
            elavated = 1
        else if (TChol >= cholrisklimit3)
            major = 1

        if (Sbp < 120 && SbpMed == 0)
            optimal = optimal + 1
        else if (Sbp >= 120 && Sbp < 140 && SbpMed == 0)
            notoptimal = notoptimal + 1
        else if (Sbp >= 140 && Sbp < 160 && SbpMed == 0)
            elavated = elavated + 1
        else if (Sbp >= 160)
            major = major + 1

        if (SbpMed == 1)
            major = major + 1

        if (Diabetes == 1)
            major = major + 1

        if (Smoker == 1)
            major = major + 1

        if (Gender == 1) {
            if (major >= 2)
                lifeTimeRisk = 69
            else if (major == 1)
                lifeTimeRisk = 50
            else if (elavated >= 1)
                lifeTimeRisk = 46
            else if (notoptimal >= 1)
                lifeTimeRisk = 36
            else if (optimal == 2)
                lifeTimeRisk = 5
            lifeTimeGoal = 5
        }
        else if (Gender == 2) {
            if (major >= 2)
                lifeTimeRisk = 50
            else if (major == 1)
                lifeTimeRisk = 39
            else if (elavated >= 1)
                lifeTimeRisk = 39
            else if (notoptimal >= 1)
                lifeTimeRisk = 27
            else if (optimal == 2)
                lifeTimeRisk = 8
            lifeTimeGoal = 8
        }
        var newALRisk = AL.replace("{0}", lifeTimeRisk + "%");
        newALRisk = newALRisk.replace("{1}", lifeTimeGoal + "%");
        $("#risk-info").html(newALRisk);
        $("#chart-info").show();
        DrawColumnChart(lifeTimeRisk, lifeTimeGoal, "option-lifetime-risk", Math.round(lifeTimeRisk / 10) * 10, optimalRiskText);
    }
    else if (selectedId == "option-risk-stroke") {
        lnage = Math.log(Age)
        lntot = Math.log(TChol)
        lnhdl = Math.log(HDL)
        if (SbpMed == 1) {
            trlnsbp = Math.log(Sbp) * 1
            ntlnsbp = Math.log(Sbp) * 0
        }
        else {
            trlnsbp = Math.log(Sbp) * 0
            ntlnsbp = Math.log(Sbp) * 1
        }
        age2 = lnage * lnage
        agetc = lnage * lntot
        agehdl = lnage * lnhdl
        agetsbp = lnage * trlnsbp
        agentsbp = lnage * ntlnsbp
        agesmoke = lnage * Smoker
        agedm = lnage * Diabetes

        goallntot = Math.log(170)
        goallnhdl = Math.log(50)
        goaltrlnsbp = 0
        goalntlnsbp = Math.log(110) * 1
        goalagetc = lnage * goallntot
        goalagehdl = lnage * goallnhdl
        goalagetsbp = lnage * goaltrlnsbp
        goalagentsbp = lnage * goalntlnsbp
        goalagesmoke = lnage * 0
        goalagedm = lnage * 0

        if (Gender == '2' && Race == '2') {
            s0_10 = 0.95334
            mnxb = 86.6081
            predict = 17.1141 * lnage + 0.9396 * lntot + (-18.9196 * lnhdl) + 4.4748 * agehdl + 29.2907 * trlnsbp + (-6.4321 * agetsbp) + 27.8197 * ntlnsbp + (-6.0873 * agentsbp) + 0.6908 * Smoker + 0.8738 * Diabetes
            goalpredict = 17.1141 * lnage + 0.9396 * goallntot + (-18.9196 * goallnhdl) + 4.4748 * goalagehdl + 29.2907 * goaltrlnsbp + (-6.4321 * goalagetsbp) + 27.8197 * goalntlnsbp + (-6.0873 * goalagentsbp) + 0.6908 * 0 + 0.8738 * 0
        }

        else if (Gender == '2' && Race != '2') {
            s0_10 = 0.96652
            mnxb = -29.1817
            predict = (-29.799 * lnage) + 4.884 * age2 + 13.54 * lntot + (-3.114 * agetc) + (-13.578 * lnhdl) + 3.149 * agehdl + 2.019 * trlnsbp + 1.957 * ntlnsbp + 7.574 * Smoker + (-1.665 * agesmoke) + 0.661 * Diabetes
            goalpredict = (-29.799 * lnage) + 4.884 * age2 + 13.54 * goallntot + (-3.114 * goalagetc) + (-13.578 * goallnhdl) + 3.149 * goalagehdl + 2.019 * goaltrlnsbp + 1.957 * goalntlnsbp + 7.574 * 0 + (-1.665 * goalagesmoke) + 0.661 * 0
        }

        else if (Gender == '1' && Race == '2') {
            s0_10 = 0.89536
            mnxb = 19.5425
            predict = 2.469 * lnage + 0.302 * lntot + (-0.307 * lnhdl) + 1.916 * trlnsbp + 1.809 * ntlnsbp + 0.549 * Smoker + 0.645 * Diabetes
            goalpredict = 2.469 * lnage + 0.302 * goallntot + (-0.307 * goallnhdl) + 1.916 * goaltrlnsbp + 1.809 * goalntlnsbp + 0.549 * 0 + 0.645 * 0
        }

        else if (Gender == '1' && Race != '2') {
            s0_10 = 0.91436
            mnxb = 61.1816
            predict = 12.344 * lnage + 11.853 * lntot + (-2.664 * agetc) + (-7.99 * lnhdl) + 1.769 * agehdl + 1.797 * trlnsbp + 1.764 * ntlnsbp + 7.837 * Smoker + (-1.795 * agesmoke) + 0.658 * Diabetes
            goalpredict = 12.344 * lnage + 11.853 * goallntot + (-2.664 * goalagetc) + (-7.99 * goallnhdl) + 1.769 * goalagehdl + 1.797 * goaltrlnsbp + 1.764 * goalntlnsbp + 7.837 * 0 + (-1.795 * goalagesmoke) + 0.658 * 0
        }

        cvdpredict = 1 - (Math.pow(s0_10, Math.exp(predict - mnxb)))
        ascvd = (cvdpredict * 100).toFixed(1)

        goalcvdpredict = 1 - (Math.pow(s0_10, Math.exp(goalpredict - mnxb)))
        goal = (goalcvdpredict * 100).toFixed(1)

        var newHSRisk = HS.replace("{0}", ascvd + "%");
        newHSRisk = newHSRisk.replace("{1}", goal + "%");
        $("#risk-info").html(newHSRisk);
        $("#chart-info").show();
        DrawColumnChart(ascvd, goal, "option-lifetime-risk", Math.round(ascvd / 10) * 10, optimalRiskText);
    }
    else {
        var riskpts;
        var CHDRISKS;
        var avgrisk;
        var lowrisk;

        riskpts = 0;

        if (Gender != "2") {
            //   step 1 - male - age
            // 30-34=-1,  35-39=0,  40-44=+1 etc
            if (Age < 35)
                riskpts = riskpts - 1;
            else
                riskpts = riskpts + Math.floor((Age - 35) / 5);
            //   step 2 - male - cholesterol
            if (TChol < cholrisklimit1) {
                riskpts = riskpts - 3;
            } if (TChol >= cholrisklimit1 && TChol < cholrisklimit2) {
                riskpts = riskpts + 0;
            } if (TChol >= cholrisklimit2 && TChol < cholrisklimit3) {
                riskpts = riskpts + 1;
            } if (TChol >= cholrisklimit3 && TChol < cholrisklimit4) {
                riskpts = riskpts + 2;
            } if (TChol >= cholrisklimit4) {
                riskpts = riskpts + 3;
            }
            //   step 3 - male - hdl
            if (HDL < hdlrisklimit1) {
                riskpts = riskpts + 2;
            } if (HDL >= hdlrisklimit1 && HDL < hdlrisklimit2) {
                riskpts = riskpts + 1;
            } if (HDL >= hdlrisklimit2 && HDL < hdlrisklimit3) {
                riskpts = riskpts + 0;
            } if (HDL >= hdlrisklimit3 && HDL < hdlrisklimit4) {
                riskpts = riskpts + 0;
            } if (HDL >= hdlrisklimit4) {
                riskpts = riskpts - 2;
            }
            //   step 4 - male - blood pressure
            if (Sbp < 120) {
                if (Dbp <= 84) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 85 && Dbp <= 89) {
                    riskpts = riskpts + 1;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 120 && Sbp <= 129) {
                if (Dbp <= 84) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 85 && Dbp <= 89) {
                    riskpts = riskpts + 1;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 130 && Sbp <= 139) {
                if (Dbp <= 89) {
                    riskpts = riskpts + 1;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 140 && Sbp <= 159) {
                if (Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 160) {
                riskpts = riskpts + 3;
            }

            //   step 5 - male - diabetes
            if (Diabetes == 1) {
                riskpts = riskpts + 2;
            }
            //   step 6 - male - smoker
            if (Smoker == 1) {
                riskpts = riskpts + 2;
            }
            //   step 8 - calculate risk - male  - chdrisks
            CHDRISKS = 0.02;
            if (riskpts == 0) { CHDRISKS = 0.03; }
            if (riskpts == 1) { CHDRISKS = 0.03; }
            if (riskpts == 2) { CHDRISKS = 0.04; }
            if (riskpts == 3) { CHDRISKS = 0.05; }
            if (riskpts == 4) { CHDRISKS = 0.07; }
            if (riskpts == 5) { CHDRISKS = 0.08; }
            if (riskpts == 6) { CHDRISKS = 0.1; }
            if (riskpts == 7) { CHDRISKS = 0.13; }
            if (riskpts == 8) { CHDRISKS = 0.16; }
            if (riskpts == 9) { CHDRISKS = 0.2; }
            if (riskpts == 10) { CHDRISKS = 0.25; }
            if (riskpts == 11) { CHDRISKS = 0.31; }
            if (riskpts == 12) { CHDRISKS = 0.37; }
            if (riskpts == 13) { CHDRISKS = 0.45; }
            if (riskpts >= 14) { CHDRISKS = 0.53; }
        } else { // END OF MALE PROCESSING  **************************
            // START FEMALE PROCESSING ...
            //   step 1 - female - age

            if (Age < 35) {
                riskpts = riskpts - 9;
            } if (Age >= 35 && Age <= 39) {
                riskpts = riskpts - 4;
            } if (Age >= 40 && Age <= 44) {
                riskpts = riskpts + 0;
            } if (Age >= 45 && Age <= 49) {
                riskpts = riskpts + 3;
            } if (Age >= 50 && Age <= 54) {
                riskpts = riskpts + 6;
            } if (Age >= 55 && Age <= 59) {
                riskpts = riskpts + 7;
            } if (Age >= 60 && Age <= 64) {
                riskpts = riskpts + 8;
            } if (Age >= 65 && Age <= 69) {
                riskpts = riskpts + 8;
            } if (Age >= 70 && Age <= 74) {
                riskpts = riskpts + 8;
            }
            //   step 2 - female - cholesterol
            if (TChol < cholrisklimit1) {
                riskpts = riskpts - 2;
            } if (TChol >= cholrisklimit1 && TChol < cholrisklimit2) {
                riskpts = riskpts + 0;
            } if (TChol >= cholrisklimit2 && TChol < cholrisklimit3) {
                riskpts = riskpts + 1;
            } if (TChol >= cholrisklimit3 && TChol < cholrisklimit4) {
                riskpts = riskpts + 1;
            } if (TChol >= cholrisklimit4) {
                riskpts = riskpts + 3;
            }
            //   step 3 - female - hdl
            if (HDL < hdlrisklimit1) {
                riskpts = riskpts + 5;
            } if (HDL >= hdlrisklimit1 && HDL < hdlrisklimit2) {
                riskpts = riskpts + 2;
            } if (HDL >= hdlrisklimit2 && HDL < hdlrisklimit3) {
                riskpts = riskpts + 1;
            } if (HDL >= hdlrisklimit3 && HDL < hdlrisklimit4) {
                riskpts = riskpts + 0;
            } if (HDL >= hdlrisklimit4) {
                riskpts = riskpts - 3;
            }
            //   step 4 - female - blood pressure
            if (Sbp < 120) {
                if (Dbp < 80) {
                    riskpts = riskpts - 3;
                }
                if (Dbp >= 80 && Dbp <= 84) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 85 && Dbp <= 89) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 120 && Sbp <= 129) {
                if (Dbp <= 89) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 130 && Sbp <= 139) {
                if (Dbp <= 89) {
                    riskpts = riskpts + 0;
                }
                if (Dbp >= 90 && Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 140 && Sbp <= 159) {
                if (Dbp <= 99) {
                    riskpts = riskpts + 2;
                }
                if (Dbp >= 100) {
                    riskpts = riskpts + 3;
                }
            }
            if (Sbp >= 160) {
                riskpts = riskpts + 3;
            }
            //   step 5 - female - diabetes
            if (Diabetes == 1) {
                riskpts = riskpts + 4;
            }
            //   step 6 - female - smoker
            if (Smoker == 1) {
                riskpts = riskpts + 2;
            }
            //   step 8 - calculate risk - female  - chdrisks
            CHDRISKS = 0.01;
            if (riskpts == -1) { CHDRISKS = 0.02; }
            if (riskpts == 0) { CHDRISKS = 0.02; }
            if (riskpts == 1) { CHDRISKS = 0.02; }
            if (riskpts == 2) { CHDRISKS = 0.03; }
            if (riskpts == 3) { CHDRISKS = 0.03; }
            if (riskpts == 4) { CHDRISKS = 0.04; }
            if (riskpts == 5) { CHDRISKS = 0.04; }
            if (riskpts == 6) { CHDRISKS = 0.05; }
            if (riskpts == 7) { CHDRISKS = 0.06; }
            if (riskpts == 8) { CHDRISKS = 0.07; }
            if (riskpts == 9) { CHDRISKS = 0.08; }
            if (riskpts == 10) { CHDRISKS = 0.1; }
            if (riskpts == 11) { CHDRISKS = 0.11; }
            if (riskpts == 12) { CHDRISKS = 0.13; }
            if (riskpts == 13) { CHDRISKS = 0.15; }
            if (riskpts == 14) { CHDRISKS = 0.18; }
            if (riskpts == 15) { CHDRISKS = 0.2; }
            if (riskpts == 16) { CHDRISKS = 0.24; }
            if (riskpts >= 17) { CHDRISKS = 0.27; }
            //   step 8 - calculate average risks
        } // end of FEMALE processing

        if (Gender == "1") {
            if (Age < 30) {
                avgrisk = 0.01;
                lowrisk = 0.01;
            }
            if (Age >= 30 && Age <= 34) {
                avgrisk = 0.03;
                lowrisk = 0.02;
            }
            if (Age >= 35 && Age <= 39) {
                avgrisk = 0.05;
                lowrisk = 0.03;
            }
            if (Age >= 40 && Age <= 44) {
                avgrisk = 0.07;
                lowrisk = 0.04;
            }
            if (Age >= 45 && Age <= 49) {
                avgrisk = 0.11;
                lowrisk = 0.04;
            }
            if (Age >= 50 && Age <= 54) {
                avgrisk = 0.14;
                lowrisk = 0.06;
            }
            if (Age >= 55 && Age <= 59) {
                avgrisk = 0.16;
                lowrisk = 0.07;
            }
            if (Age >= 60 && Age <= 64) {
                avgrisk = 0.21;
                lowrisk = 0.09;
            }
            if (Age >= 65 && Age <= 69) {
                avgrisk = 0.25;
                lowrisk = 0.11;
            }
            if (Age >= 70 && Age <= 74) {
                avgrisk = 0.3;
                lowrisk = 0.14;
            }
        } else {    // female
            if (Age < 30) {
                avgrisk = 0.01;
                lowrisk = 0.01;
            }
            if (Age >= 30 && Age <= 34) {
                avgrisk = 0.01;
                lowrisk = 0.01;
            }
            if (Age >= 35 && Age <= 39) {
                avgrisk = 0.01;
                lowrisk = 0.01;
            }
            if (Age >= 40 && Age <= 44) {
                avgrisk = 0.02;
                lowrisk = 0.02;
            }
            if (Age >= 45 && Age <= 49) {
                avgrisk = 0.05;
                lowrisk = 0.03;
            }
            if (Age >= 50 && Age <= 54) {
                avgrisk = 0.08;
                lowrisk = 0.05;
            }
            if (Age >= 55 && Age <= 59) {
                avgrisk = 0.12;
                lowrisk = 0.07;
            }
            if (Age >= 60 && Age <= 64) {
                avgrisk = 0.12;
                lowrisk = 0.08;
            }
            if (Age >= 65 && Age <= 69) {
                avgrisk = 0.13;
                lowrisk = 0.08;
            }
            if (Age >= 70 && Age <= 74) {
                avgrisk = 0.14;
                lowrisk = 0.08;
            }
        }
        pCurrentFram = (CHDRISKS * 100).toFixed(0);
        pAverageFram = avgrisk * 100;
        pLowFram = (lowrisk * 100).toFixed(0);

        if (selectedId == "option-risk-heart-disease") {
            var newchdRisk = chdRisk.replace("{0}", pCurrentFram + "%");
            newchdRisk = newchdRisk.replace("{1}", pLowFram + "%");
            $("#risk-info").html(newchdRisk);
            $("#chart-info").show();
            DrawColumnChart(Math.round(pCurrentFram), Math.round(pLowFram), "option-risk-heart-disease", Math.round(pCurrentFram / 10) * 10, lowRiskText);
        }
        else {
            var heartAge = "";
            var vh;
            var vw;
            var heartAgeNum;

            if (!isNaN(pCurrentFram)) {
                vw = parseInt(pCurrentFram.toString(), 10);
                if (Gender == "1")	// Male
                {
                    vh = "0";
                    if (vw <= 2) {
                        if (Age < 30)
                            vh = Age.toString();
                        else
                            vh = "30 or under";
                    }
                    if (vw > 14) vw = 99;
                    if (vh == "0") {
                        if (vw == 2) vh = "30";
                        if (vw == 3) vh = "35";
                        if (vw == 4) vh = "40";
                        if (vw == 5) vh = "45";
                        if (vw == 6) vh = "50";
                        if (vw == 7) vh = "55";
                        if (vw == 8) vh = "57";
                        if (vw == 9) vh = "60";
                        if (vw == 10) vh = "62";
                        if (vw == 11) vh = "65";
                        if (vw == 12) vh = "67";
                        if (vw == 13) vh = "68";
                        if (vw == 14) vh = "70";
                        if (vw == 99) vh = "over 74";
                    }
                }
                else		// Female
                {
                    vh = "0";
                    if (vw <= 1) {
                        if (Age < 35)
                            vh = Age.toString();
                        else
                            vh = "35 or under";
                    }
                    if (vw > 8) vw = 99;
                    if (vh == "0") {
                        if (vw == 1) vh = "35";
                        if (vw == 2) vh = "40";
                        if (vw == 3) vh = "45";
                        if (vw == 4) vh = "47";
                        if (vw == 5) vh = "50";
                        if (vw == 6) vh = "52";
                        if (vw == 7) vh = "55";
                        if (vw == 8) vh = "60";
                        if (vw == 99) vh = "over 74";
                    }
                }
                heartAge = vh;
            }
            if (vh == "over 74") heartAge = "75";
            heartAgeNum = heartAge;
            if (heartAge == "30 or under")
                heartAgeNum = "30";
            if (heartAge == "35 or under")
                heartAgeNum = "35";

            if (heartAgeNum != "0" && !isNaN(heartAgeNum)) {
                var vHAMin = "0";		// dial minimum
                var vHAMax = "100";	// dial maximum
                if (Age > 25 && Age < 65) {
                    var greenFrom = vHAMin;
                    var greenTo = Age.toString();
                    var yellowFrom = Age.toString();
                    var vx = parseInt(Age, 10) + 10;
                    var yellowTo = vx.toString();
                    var redFrom = vx.toString();
                    var redTo = vHAMax;
                }
                if (Age <= 25) {
                    var greenFrom = vHAMin;
                    var greenTo = 25;
                    var yellowFrom = 25;
                    var yellowTo = 35;
                    var redFrom = 35;
                    var redTo = vHAMax;
                }
                if (Age >= 65) {
                    var greenFrom = vHAMin;
                    var greenTo = Age;
                    var yellowFrom = Age;
                    var yellowTo = Age;
                    var redFrom = Age;
                    var redTo = vHAMax;
                }
                var newHA;
                if (heartAgeNum != "") {
                    if (heartAgeNum == "74")
                        newHA = over74HA.replace("{0}", heartAgeNum);
                    else
                        newHA = HA.replace("{0}", heartAgeNum);
                }
                $("#risk-info").html(newHA);
                $("#chart-info").show();
                DrawGauge(heartAgeNum, greenTo, yellowTo, redTo);
            }
        }
    }
}

function DrawColumnChart(current, goal, chartName, max, goalText) {
    if (parseFloat(goal) < parseFloat(current))
        currentcolor = "#ff3939";
    else
        currentcolor = "#2fff2f";    
    am4core.addLicense("CH39169069");
    var chart = am4core.create("chart", am4charts.XYChart);
    chart.data = [{
        "risk": currentRiskText,
        "percentage": current,
        "color": currentcolor
    }, {
        "risk": goalText,
        "percentage": goal,
        "color": "#20698a"
    }];

    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;


    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    //series.tooltipHTML = "{category}: <b>{value}</b>";
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.text = "{percentage}%";
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;

    chart.maskBullets = false;
    $('.item-label > .value').text(current + "%");
    return chart;
}

function DrawGauge(heartAgeNum, greenTo, yellowTo, redTo) {
    am4core.addLicense("CH39169069");
    var heartAgeNumText;
    if (heartAgeNum > 74)
        heartAgeNumText = "74+";
    else
        heartAgeNumText = heartAgeNum;    
    // create chart
    var chart = am4core.create("chart", am4charts.GaugeChart);
    chart.hiddenState.properties.opacity = 0; // this makes initial fade in effect

    chart.innerRadius = -15;

    var axis = chart.xAxes.push(new am4charts.ValueAxis());
    axis.min = 1;
    axis.max = 100;
    axis.strictMinMax = true;
    axis.renderer.grid.template.stroke = new am4core.InterfaceColorSet().getFor("background");
    axis.renderer.grid.template.strokeOpacity = 0.3;
    axis.renderer.minGridDistance = 100;

    var range0 = axis.axisRanges.create();
    range0.value = 0;
    range0.endValue = greenTo;
    range0.axisFill.fillOpacity = 1;
    range0.axisFill.fill = "#2fff2f";
    range0.axisFill.zIndex = - 1;

    var range1 = axis.axisRanges.create();
    range1.value = greenTo;
    range1.endValue = yellowTo;
    range1.axisFill.fillOpacity = 1;
    range1.axisFill.fill = "#ffff63";
    range1.axisFill.zIndex = -1;

    var range2 = axis.axisRanges.create();
    range2.value = yellowTo;
    range2.endValue = redTo;
    range2.axisFill.fillOpacity = 1;
    range2.axisFill.fill = "#ff3939";
    range2.axisFill.zIndex = -1;

    var label1 = chart.chartContainer.createChild(am4core.Label);
    label1.text = heartAgeText;
    label1.align = "center";


    var label = chart.radarContainer.createChild(am4core.Label);
    label.isMeasured = false;
    label.fontSize = 30;
    label.x = am4core.percent(50);
    label.y = am4core.percent(100);
    label.horizontalCenter = "middle";
    label.verticalCenter = "bottom";
    label.text = parseInt(heartAgeNum);



    var hand = chart.hands.push(new am4charts.ClockHand());
    hand.axis = axis;
    hand.innerRadius = am4core.percent(20);
    hand.startWidth = 10;
    hand.pin.disabled = true;
    hand.value = parseInt(heartAgeNum);

    var value = parseInt(heartAgeNum);
    label.text = value;
    var animation = new am4core.Animation(hand, {
        property: "value",
        to: value
    }, 1000, am4core.ease.cubicOut).start();


    $('.item-label > .value').text(heartAgeNumText);
    return chart;
}

function clearChart() {
    $("#chart").html('');
}